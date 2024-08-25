using VeloBrawl.V2.VB24BXIM9CA.Auxiliary.Cleaner;
using VeloBrawl.V2.VB24BXIM9CA.Auxiliary.Settings;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects.Enumerations.Secure;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects.HelpDirectory;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects.OwnLogging;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects.OwnLogging.MiniHelper;

namespace VeloBrawl.V2.VB24BXIM9CA.Auxiliary;

public static class Program
{
    public static void Main(string[] args)
    {
        AppConfigurator.InAppGlobalLogLevel = (UniqueLogLevels)AppConfig.LogSensitive;

        AppConfigurator.LogoConstant =
            "\n\u2591\u2588\u2588\u2588\u2588\u2588\u2557\u2591\u2591\u2588\u2588\u2588\u2588\u2588\u2557\u2591\u2588\u2588\u2588\u2588\u2588\u2588\u2557\u2591\u2588\u2588\u2588\u2588\u2588\u2588\u2588\u2557\n\u2588\u2588\u2554\u2550\u2550\u2588\u2588\u2557\u2588\u2588\u2554\u2550\u2550\u2588\u2588\u2557\u2588\u2588\u2554\u2550\u2550\u2588\u2588\u2557\u2588\u2588\u2554\u2550\u2550\u2550\u2550\u255d\n\u2588\u2588\u2551\u2591\u2591\u255a\u2550\u255d\u2588\u2588\u2551\u2591\u2591\u2588\u2588\u2551\u2588\u2588\u2588\u2588\u2588\u2588\u2554\u255d\u2588\u2588\u2588\u2588\u2588\u2557\u2591\u2591\n\u2588\u2588\u2551\u2591\u2591\u2588\u2588\u2557\u2588\u2588\u2551\u2591\u2591\u2588\u2588\u2551\u2588\u2588\u2554\u2550\u2550\u2588\u2588\u2557\u2588\u2588\u2554\u2550\u2550\u255d\u2591\u2591\n\u255a\u2588\u2588\u2588\u2588\u2588\u2554\u255d\u255a\u2588\u2588\u2588\u2588\u2588\u2554\u255d\u2588\u2588\u2551\u2591\u2591\u2588\u2588\u2551\u2588\u2588\u2588\u2588\u2588\u2588\u2588\u2557\n\u2591\u255a\u2550\u2550\u2550\u2550\u255d\u2591\u2591\u255a\u2550\u2550\u2550\u2550\u255d\u2591\u255a\u2550\u255d\u2591\u2591\u255a\u2550\u255d\u255a\u2550\u2550\u2550\u2550\u2550\u2550\u255d";
        LogoGroundDrawer.DrawLogoLines();

        GcManualCollector.StartCollector(AppConfig.MinutesToGc);
        AppEnvironment.PathToSavedFiles = HelperCity.FixAutoimmuneFilePath(AppDomain.CurrentDomain.BaseDirectory);

        if ((int)AppConfigurator.InAppGlobalLogLevel <= 1) TextProgressBarGroundDrawer.DrawTextProgressBar(8, 10, 100);
        ConsoleLogger.WriteTextWithPrefix(ConsoleLogger.Prefixes.Cmd, "Hi new project! <- Auxiliary: Hello!");
    }
}