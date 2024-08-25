using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects.OwnLogging.MiniHelper;

namespace VeloBrawl.V2.VB24BXIM9CA.Core;

public static class Program
{
    public static void Main(string[] args)
    {
        Auxiliary.Program.Main(args);
        AnalyticLayer.Program.Main(args);
        GameChannel.Program.Main(args);
        GameProxy.Program.Main(args);
        RabbitMqProxy.Program.Main(args);
        RabbitMqFirewall.Program.Main(args);

        DependencyManualInjector.Build();
        Thread.Sleep(500);

        if ((int)AppConfigurator.InAppGlobalLogLevel <= 1)
        {
            TextProgressBarGroundDrawer.DrawTextProgressBar(8, 100, 100);
            Console.WriteLine("\nVeloBrawl.V2.VB24BXIM9CA.Core started!");
        }

        for (;;) ;
    }
}