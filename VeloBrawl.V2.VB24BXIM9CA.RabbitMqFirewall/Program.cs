using Microsoft.Extensions.DependencyInjection;
using VeloBrawl.V2.VB24BXIM9CA.AbstractionLayer.Interfaces;
using VeloBrawl.V2.VB24BXIM9CA.RabbitMqFirewall.In;
using VeloBrawl.V2.VB24BXIM9CA.RabbitMqFirewall.Out;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects.OwnLogging;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects.OwnLogging.MiniHelper;

namespace VeloBrawl.V2.VB24BXIM9CA.RabbitMqFirewall;

public static class Program
{
    public static void Main(string[] args)
    {
        var rabbitIn = new RabbitInFirewall();
        rabbitIn.Create();

        var rabbitOut = new RabbitOutFirewall();
        rabbitOut.Create();

        DependencyManualInjector.ServiceCollection.AddSingleton<IRabbitOut>(rabbitOut);
        ConsoleLogger.WriteTextWithPrefix(ConsoleLogger.Prefixes.Debug, "RabbitMQ firewall started!");

        if ((int)AppConfigurator.InAppGlobalLogLevel <= 1) TextProgressBarGroundDrawer.DrawTextProgressBar(8, 95, 100);
        ConsoleLogger.WriteTextWithPrefix(ConsoleLogger.Prefixes.Cmd, "Hi new project! <- RabbitMqFirewall: Hello!");
    }
}