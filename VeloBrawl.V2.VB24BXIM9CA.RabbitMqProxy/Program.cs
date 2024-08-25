using VeloBrawl.V2.VB24BXIM9CA.RabbitMqProxy.Consu;
using VeloBrawl.V2.VB24BXIM9CA.RabbitMqProxy.Produ;
using VeloBrawl.V2.VB24BXIM9CA.RabbitMqProxy.Shell;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects.OwnLogging;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects.OwnLogging.MiniHelper;

namespace VeloBrawl.V2.VB24BXIM9CA.RabbitMqProxy;

public static class Program
{
    private static readonly int[] Nodes = [1, 3, 4, 6, 7, 9, 10, 11, 13, 23, 25, 26, 27, 30, 33, 57];

    public static void Main(string[] args)
    {
        foreach (var node in Nodes)
        {
            var c = new NodeServerConsumer($"VeloBrawl.c.node_{node}"); // core

            c.Create();
            c.ConsumeMessages((_, ea) => NodeServerProcessor.ReceiveMessageFromNode(ea.Body.ToArray()));

            ConsoleLogger.WriteTextWithPrefix(ConsoleLogger.Prefixes.Debug, $"Starting consumer for node {node}!");
        }

        Thread.Sleep(1000);
        ConsoleLogger.WriteTextWithPrefix(ConsoleLogger.Prefixes.Cmd, "Waiting for RabbitMQ normalization...");
        if ((int)AppConfigurator.InAppGlobalLogLevel <= 1) TextProgressBarGroundDrawer.DrawTextProgressBar(8, 85, 100);
        Thread.Sleep(1500);

        foreach (var node in Nodes)
        {
            var p = new NodeServerProducer($"VeloBrawl.i.node_{node}"); // injected

            p.Create();
            NodeServerProcessor.AddNodeServerProducer(node, p);

            ConsoleLogger.WriteTextWithPrefix(ConsoleLogger.Prefixes.Debug, $"Starting producer for node {node}!");
        }

        if ((int)AppConfigurator.InAppGlobalLogLevel <= 1) TextProgressBarGroundDrawer.DrawTextProgressBar(8, 90, 100);
        ConsoleLogger.WriteTextWithPrefix(ConsoleLogger.Prefixes.Cmd, "Hi new project! <- RabbitMqProxy: Hello!");
    }
}