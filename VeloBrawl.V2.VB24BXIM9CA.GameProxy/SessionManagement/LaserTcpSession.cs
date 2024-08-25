using Microsoft.Extensions.DependencyInjection;
using VeloBrawl.V2.VB24BXIM9CA.AbstractionLayer.Interfaces;
using VeloBrawl.V2.VB24BXIM9CA.AnalyticLayer.OnlineAnalytics;
using VeloBrawl.V2.VB24BXIM9CA.GameProxy.GameMessagingPart;
using VeloBrawl.V2.VB24BXIM9CA.RabbitMqProxy.Shell;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects.BaseNetworking;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects.BaseNetworking.Slave;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects.EngineFactory.MathematicalSector;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects.HelpDirectory;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects.InGameUtilities;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects.OwnLogging;
using Timer = System.Timers.Timer;

namespace VeloBrawl.V2.VB24BXIM9CA.GameProxy.SessionManagement;

public class LaserTcpSession(TcpServer tcpServer, ITcpSocketServer tcpSocketInterface)
    : TcpSession(tcpServer), ITcpSocketSession
{
    private int _keepAliveLastTick;
    private bool _secureAdministrated;

    private Timer _toDiscTimer = null!;
    private bool _toDiscTimerCanBeEliminatedByNewMessage;

    private Timer Timer { get; set; } = null!;

    public TcpSession Session { get; set; } = null!;
    public IMessaging Messaging { get; set; } = null!;

    protected override void OnConnected()
    {
        if (!_secureAdministrated) Administrate();
        base.OnConnected();

        try
        {
            ConsoleLogger.WriteTextWithPrefix(ConsoleLogger.Prefixes.Tcp, $"New TcpSocketSession created! " +
                                                                          $"Information: Id = {Id}; Ip = {HelperCity.GetIpBySocket(Socket)}. " +
                                                                          $"TcpServer info: Ip = {Server.Address}; Port = {Server.Port}. " +
                                                                          $"Status: num of sessions on this port: {++tcpSocketInterface.NumberOfActiveConnections}");
        }
        catch
        {
            ConsoleLogger.WriteTextWithPrefix(ConsoleLogger.Prefixes.Cmd,
                $"DOS warning on server: Ip = {Server.Address}; Port = {Server.Port}.");
        }

        tcpSocketInterface.NumberOfAllConnections++;
        DisconnectAfterTime(5, true);
    }

    protected override void OnDisconnected()
    {
        base.OnDisconnected();
        UnAdministrate();

        try
        {
            ConsoleLogger.WriteTextWithPrefix(ConsoleLogger.Prefixes.Tcp, $"TcpSocketSession closed! " +
                                                                          $"Information: Id = {Id}. " +
                                                                          $"TcpServer info: Ip = {Server.Address}; Port = {Server.Port}. " +
                                                                          $"Status: num of sessions on this port: {--tcpSocketInterface.NumberOfActiveConnections}");

            if (tcpSocketInterface.NumberOfActiveConnections < 0)
                ConsoleLogger.WriteTextWithPrefix(ConsoleLogger.Prefixes.Cmd,
                    $"SYN-Flood warning on server: Ip = {Server.Address}; Port = {Server.Port}. Fatality!!!");
        }
        catch
        {
            ConsoleLogger.WriteTextWithPrefix(ConsoleLogger.Prefixes.Cmd,
                $"DDOS warning on server: Ip = {Server.Address}; Port = {Server.Port}.");
        }

        tcpSocketInterface.NumberOfActiveConnections =
            LogicMath.Clamp(tcpSocketInterface.NumberOfActiveConnections, 0, 100_000_000); // SYN-Flood protector
    }

    protected override void OnReceived(byte[] buffer, long offset, long size)
    {
        if (size + offset >= 2048)
        {
            Disconnect();
            return;
        }

        if (!_secureAdministrated) Administrate();
        base.OnReceived(buffer, offset, size);

        if (!_secureAdministrated) return;

        if (Messaging.NextMessage(buffer, offset, size, out _) < 0)
            Disconnect();

        if (!_toDiscTimerCanBeEliminatedByNewMessage || _toDiscTimer == null!) return;
        _toDiscTimer.Stop();
        _toDiscTimer.Dispose();
        _toDiscTimer = null!;
    }

    public void KeepAlive()
    {
        _keepAliveLastTick = LogicTimeUtil.GetTimestamp();
    }

    public void DisconnectAfterTime(int secs, bool canBeEliminatedByNewMessage = false)
    {
        _toDiscTimer = new Timer(1000 * secs) { Enabled = true, AutoReset = false };
        _toDiscTimerCanBeEliminatedByNewMessage = canBeEliminatedByNewMessage;

        _toDiscTimer.Elapsed += (_, _) =>
        {
            Disconnect();

            if (_toDiscTimer == null!) return;
            _toDiscTimer.Dispose();
            _toDiscTimer = null!;
        };
    }

    private void Administrate()
    {
        if (_secureAdministrated) return;

        Timer = new Timer(1000 * 60) { Enabled = true, AutoReset = true };
        {
            Timer.Elapsed += (_, _) =>
            {
                if (LogicTimeUtil.GetTimestamp() - _keepAliveLastTick > 40) Disconnect();
                // TODO: more checks
            };
        }

        _secureAdministrated = true;
        _keepAliveLastTick = LogicTimeUtil.GetTimestamp();

        Session = this;
        Messaging = new Messaging(this);

        NodeServerProcessor.AddTcpSocketSession(Id, this);

        if (OnlineAnalytic.GetOnlineAnalytic() == null!) return;
        OnlineAnalytic.GetOnlineAnalytic().AddPlayer();
    }

    private void UnAdministrate()
    {
        if (!_secureAdministrated) return;

        if (Timer != null!)
        {
            Timer.Stop();
            Timer.Dispose();
            Timer = null!;
        }

        if (Messaging.LoginReceived)
            DependencyManualInjector.ServiceProvider
                .GetRequiredService<IRabbitOut>().SendMessage("CLOSE " + Id);

        Session = null!;
        Messaging = null!;

        NodeServerProcessor.RemoveTcpSocketSession(Id);

        _toDiscTimer = null!;
        _toDiscTimerCanBeEliminatedByNewMessage = false;

        Dispose();
    }
}