using VeloBrawl.V2.VB24BXIM9CA.GameProxy.ProtectionsLayer;
using VeloBrawl.V2.VB24BXIM9CA.RabbitMqProxy.Shell;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects.HelpDirectory;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects.OwnLogging;

namespace VeloBrawl.V2.VB24BXIM9CA.RabbitMqFirewall.Lexical;

public static class LexicalAnalyzer
{
    public static void AnalyzeAndProcess(string input)
    {
        try
        {
            var tokens = input.Split(' ');

            switch (tokens[0])
            {
                case "CLOSE":
                {
                    var guid = new Guid(tokens[1]);

                    var session = NodeServerProcessor.GetTcpSocketSession(guid);
                    if (session == null) return;

                    session.Session.Disconnect();
                    break;
                }

                case "BAN_C1":
                {
                    var guid = new Guid(tokens[1]);

                    var session = NodeServerProcessor.GetTcpSocketSession(guid);
                    if (session == null) return;

                    ProtectionStatic.BannedIpAddresses.TryAdd(HelperCity.GetIpBySocket(session.Session.Socket)!, true);
                    break;
                }

                case "BAN_C2":
                {
                    var guid = new Guid(tokens[1]);

                    var session = NodeServerProcessor.GetTcpSocketSession(guid);
                    if (session == null) return;

                    ProtectionStatic.BannedIpAddresses.TryAdd(HelperCity.GetIpBySocket(session.Session.Socket)!, true);
                    session.Session.Disconnect();
                    break;
                }

                case "BAN_D":
                {
                    ProtectionStatic.BannedIpAddresses.TryAdd(tokens[1], true);
                    break;
                }

                case "UNBAN_C1":
                {
                    var guid = new Guid(tokens[1]);

                    var session = NodeServerProcessor.GetTcpSocketSession(guid);
                    if (session == null) return;

                    ProtectionStatic.BannedIpAddresses.TryRemove(HelperCity.GetIpBySocket(session.Session.Socket)!,
                        out _);
                    break;
                }

                case "UNBAN_C2":
                {
                    // unreal...
                    break;
                }

                case "UNBAN_D":
                {
                    ProtectionStatic.BannedIpAddresses.TryRemove(tokens[1], out _);
                    break;
                }
            }
        }
        catch (Exception e)
        {
            ConsoleLogger.WriteTextWithPrefix(ConsoleLogger.Prefixes.Error, e);
        }
    }
}