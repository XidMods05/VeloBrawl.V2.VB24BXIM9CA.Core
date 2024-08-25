using System.Runtime.Serialization.Formatters.Binary;
using Timer = System.Timers.Timer;

namespace VeloBrawl.V2.VB24BXIM9CA.AnalyticLayer.OnlineAnalytics;

public static class OnlineAnalytic
{
    private static string _path = null!;
    private static OnlineAnalyticIns _onlineAnalyticIns = null!;

    public static void Init(string path)
    {
        _path = path;

        if (!File.ReadLines(path).Any())
        {
            _onlineAnalyticIns = new OnlineAnalyticIns();
            {
                for (var i = 0; i < 24; i++) _onlineAnalyticIns.OnlinePerHour.TryAdd(i, 0);
                for (var i = 0; i < 7; i++) _onlineAnalyticIns.OnlinePerDayOfWeek.TryAdd(i, 0);
                for (var i = 0; i < 31; i++) _onlineAnalyticIns.OnlinePerMonth.TryAdd(i, 0);
                for (var i = 2020; i < 2050; i++) _onlineAnalyticIns.OnlinePerYear.TryAdd(i, 0);
            }

            SecureAddToBin();
        }
        else
        {
            SecureLoadFromBin();
        }

        new Timer(1000 * 60 * 10) { Enabled = true, AutoReset = true }
            .Elapsed += (_, _) => SecureAddToBin();
    }

    private static void SecureLoadFromBin()
    {
        using Stream stream = File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        _onlineAnalyticIns = (OnlineAnalyticIns)new BinaryFormatter().Deserialize(stream);
        stream.Close();
    }

    private static void SecureAddToBin()
    {
        using var stream = File.Open(_path, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
        new BinaryFormatter().Serialize(stream, _onlineAnalyticIns);
        stream.Close();
    }

    public static OnlineAnalyticIns GetOnlineAnalytic()
    {
        return _onlineAnalyticIns;
    }
}