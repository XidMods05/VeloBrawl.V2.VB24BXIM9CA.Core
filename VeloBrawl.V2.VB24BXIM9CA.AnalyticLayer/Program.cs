using VeloBrawl.V2.VB24BXIM9CA.AnalyticLayer.OnlineAnalytics;
using VeloBrawl.V2.VB24BXIM9CA.Auxiliary.Settings;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects.OwnLogging;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects.OwnLogging.MiniHelper;

namespace VeloBrawl.V2.VB24BXIM9CA.AnalyticLayer;

public static class Program
{
    public static void Main(string[] args)
    {
        OnlineAnalytic.Init(AppEnvironment.PathToSavedFiles + "SaveBase/analytic_online.bin");

        if ((int)AppConfigurator.InAppGlobalLogLevel <= 1) TextProgressBarGroundDrawer.DrawTextProgressBar(8, 35, 100);
        ConsoleLogger.WriteTextWithPrefix(ConsoleLogger.Prefixes.Cmd, "Hi new project! <- AnalyticLayer: Hello!");
    }
}