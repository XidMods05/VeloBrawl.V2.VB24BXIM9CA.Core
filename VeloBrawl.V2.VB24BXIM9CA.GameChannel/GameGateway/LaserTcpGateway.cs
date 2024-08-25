using System.Net;
using System.Net.Sockets;
using VeloBrawl.V2.VB24BXIM9CA.AbstractionLayer.Interfaces;
using VeloBrawl.V2.VB24BXIM9CA.Auxiliary.Settings;
using VeloBrawl.V2.VB24BXIM9CA.GameProxy.SessionManagement;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects.BaseNetworking;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects.BaseNetworking.Slave;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects.OwnLogging;
using Timer = System.Timers.Timer;

namespace VeloBrawl.V2.VB24BXIM9CA.GameChannel.GameGateway;

public class LaserTcpGateway : TcpServer, ITcpSocketServer
{
    public LaserTcpGateway(IPEndPoint endpoint) : base(endpoint)
    {
        LaserTcpGatewayBasicCtor();
    }

    public LaserTcpGateway(IPAddress address, int port) : base(address, port)
    {
        LaserTcpGatewayBasicCtor();
    }

    public LaserTcpGateway(string address, int port) : base(address, port)
    {
        LaserTcpGatewayBasicCtor();
    }

    public int NumberOfAllConnections { get; set; }
    public int NumberOfActiveConnections { get; set; }

    private void LaserTcpGatewayBasicCtor()
    {
        OptionReceiveBufferSize = 3000;
        OptionNoDelay = true;
    }

    public override bool Start()
    {
        var r = base.Start();
        ConsoleLogger.WriteTextWithPrefix(ConsoleLogger.Prefixes.Start,
            $"New LaserTcpGateway started! Listening endpoint: {Endpoint}.");
        return r;
    }

    protected override TcpSession CreateSession()
    {
        FloodManage();
        return new LaserTcpSession(this, this);
    }

    protected override void OnError(SocketError error)
    {
        base.OnError(error);
        ConsoleLogger.WriteTextWithPrefix(ConsoleLogger.Prefixes.Error,
            $"New error handled in TcpSocketListener-({Endpoint})! Error: {error}.");
    }

    private void FloodManage()
    {
        if (NumberOfAllConnections < AppConfig.MaxAllConnectionsOnTcpGateway)
            return;

        Stop(AppConfig.CloseActiveSessionsIfTcpGatewayIsClosed);

        NumberOfAllConnections = 0;
        NumberOfActiveConnections = 0;

        var t = new Timer(1000 * 100) { Enabled = true, AutoReset = false };
        {
            t.Elapsed += (_, _) =>
            {
                Start();

                t.Dispose();
                t = null!;
            };
        }
    }
}