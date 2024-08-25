using Timer = System.Timers.Timer;

namespace VeloBrawl.V2.VB24BXIM9CA.Auxiliary.Cleaner;

public static class GcManualCollector
{
    public static void StartCollector(int minToCollect)
    {
        new Timer(1000 * 60 * minToCollect) { Enabled = true, AutoReset = true }.Elapsed += (_, _) => CollectGarbage();
    }

    public static void CollectGarbage()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect(0, GCCollectionMode.Forced);
        GC.Collect(1, GCCollectionMode.Forced);
        GC.Collect(2, GCCollectionMode.Aggressive);
    }
}