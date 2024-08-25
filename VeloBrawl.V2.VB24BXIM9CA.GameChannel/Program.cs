using System.Net;
using Microsoft.Extensions.DependencyInjection;
using VeloBrawl.V2.VB24BXIM9CA.AbstractionLayer.Interfaces;
using VeloBrawl.V2.VB24BXIM9CA.Auxiliary.Settings;
using VeloBrawl.V2.VB24BXIM9CA.GameChannel.GameGateway;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects.OwnLogging;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects.OwnLogging.MiniHelper;

namespace VeloBrawl.V2.VB24BXIM9CA.GameChannel;

public static class Program
{
    public static void Main(string[] args)
    {
        foreach (var v1 in GetTcpPortsByStaticInt(AppConfig.NumberOfGenerateTcpPorts,
                     AppConfig.ConstantValueForGenerateTcpPorts).Select(tcpPort =>
                     new LaserTcpGateway(new IPEndPoint(IPAddress.Any, tcpPort))))
            if (v1.Start())
                DependencyManualInjector.ServiceCollection.AddKeyedSingleton<ITcpSocketServer>(v1.Port, v1);

        Thread.Sleep(1000);
        {
            var specific = new LaserTcpGateway(new IPEndPoint(IPAddress.Any, 9339));
            specific.Start();
            DependencyManualInjector.ServiceCollection.AddKeyedSingleton<ITcpSocketServer>(specific.Port, specific);
        }

        if ((int)AppConfigurator.InAppGlobalLogLevel <= 1) TextProgressBarGroundDrawer.DrawTextProgressBar(8, 80, 100);
        ConsoleLogger.WriteTextWithPrefix(ConsoleLogger.Prefixes.Cmd, "Hi new project! <- GameChannel: Hello!");
    }

    public static List<int> GetTcpPortsByStaticInt(int count, int staticInt)
    {
        var l = new List<int>();
        {
            for (var i = 0; i < count; i++)
                if (i % 2 == 0)
                    l.Add(staticInt - i + count - 1);
                else if (i % 5 == 0)
                    l.Add(staticInt + i - count + 500 * (i + 2) - 1);
                else
                    l.Add(staticInt - i - count + 132 * (i + 1) - 1);
        }

        return l;
    }
}