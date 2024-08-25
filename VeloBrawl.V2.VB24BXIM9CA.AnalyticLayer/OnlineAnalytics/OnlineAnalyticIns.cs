using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using VeloBrawl.V2.VB24BXIM9CA.Auxiliary.Settings;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects.EngineFactory.MathematicalSector;

namespace VeloBrawl.V2.VB24BXIM9CA.AnalyticLayer.OnlineAnalytics;

[Serializable]
public class OnlineAnalyticIns
{
    public readonly Dictionary<int, int> OnlinePerDayOfWeek = new();
    public readonly Dictionary<int, int> OnlinePerHour = new();
    public readonly Dictionary<int, int> OnlinePerMonth = new();
    public readonly Dictionary<int, int> OnlinePerYear = new();

    public void AddPlayer()
    {
        var mskNow =
            TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));

        lock (OnlinePerHour)
        {
            OnlinePerHour[mskNow.Hour]++;
        }

        lock (OnlinePerDayOfWeek)
        {
            OnlinePerDayOfWeek[(int)mskNow.DayOfWeek]++;
        }

        lock (OnlinePerMonth)
        {
            OnlinePerMonth[mskNow.Month]++;
        }

        lock (OnlinePerYear)
        {
            OnlinePerYear[mskNow.Year]++;
        }
    }

    [SuppressMessage("Interoperability", "CA1416")]
    public void DrawAnalytics(Dictionary<int, int> statistics, string analyticName)
    {
        var visitors = statistics.Select(x => x.Value).Skip(LogicMath.Max(0, statistics.Count - 24)).ToList();
        var maxValue = visitors.Count != 0 ? visitors.Max() : 0;

        var random = new Random();

        var labelFont = new Font("Arial", 10);
        var pen = new Pen(Color.Black, 1);

        const int startX = 50;
        const int startY = 500;

        const int barWidth = 25;
        const int barSpacing = 10;

        var bitmap = new Bitmap(950, 600);
        var graphics = Graphics.FromImage(bitmap);

        graphics.Clear(Color.FromArgb(240, 240, 240));
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        for (var i = 0; i < visitors.Count; i++)
        {
            var barHeight = (int)Math.Round((double)visitors[i] / maxValue * 400);

            graphics.DrawRectangle(new Pen(Color.Black, 4), startX + i * (barWidth + barSpacing), startY - barHeight,
                barWidth, barHeight);
            graphics.FillRectangle(new LinearGradientBrush(
                new Point(startX + i * (barWidth + barSpacing), startY),
                new Point(startX + i * (barWidth + barSpacing), startY - barHeight),
                Color.FromArgb(random.Next(255), random.Next(255), 120),
                Color.FromArgb(random.Next(255), 144, random.Next(255))
            ), startX + i * (barWidth + barSpacing), startY - barHeight, barWidth, barHeight);
        }

        for (var i = 0; i < visitors.Count; i += 3)
            graphics.DrawString(statistics.ElementAt(i).Key.ToString(), labelFont, Brushes.Black,
                startX + i * (barWidth + barSpacing) + 5,
                startY + 10);

        for (var i = 1; i < 10; i++)
            graphics.DrawLine(new Pen(Color.Gray, 1), startX, startY - i * 41,
                startX + visitors.Count * (barWidth + barSpacing), startY - i * 41);

        for (var i = 10; i >= 0; i--)
            graphics.DrawString((i * maxValue / 10).ToString(), labelFont, Brushes.Black,
                startX - (maxValue.ToString().Length > 6 ? 45 : 45 - 10) - maxValue.ToString().Length,
                startY - i * 40 + 5);

        graphics.DrawLine(pen, startX, startY, startX + 850, startY);
        graphics.DrawLine(pen, startX, startY, startX, startY - 400);

        graphics.DrawString("Online Analytics", new Font("Arial", 16, FontStyle.Bold), Brushes.Black, startX + 333,
            startY - 430);
        graphics.DrawString("Hour", new Font("Arial", 16, FontStyle.Bold), Brushes.Black, startX + 400, startY + 40);

        bitmap.Save(AppEnvironment.PathToSavedFiles + $"SaveBase/analytic_img_{analyticName}.png", ImageFormat.Png);
        {
            visitors.Clear();
            bitmap.Dispose();
            graphics.Dispose();
            labelFont.Dispose();
            pen.Dispose();
        }
    }
}