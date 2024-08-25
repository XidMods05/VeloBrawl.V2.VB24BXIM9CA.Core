using VeloBrawl.V2.VB24BXIM9CA.AbstractionLayer.Interfaces;
using VeloBrawl.V2.VB24BXIM9CA.Auxiliary.Debug;
using VeloBrawl.V2.VB24BXIM9CA.GameProxy.SessionManagement;
using VeloBrawl.V2.VB24BXIM9CA.RabbitMqProxy.Shell;
using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects.OwnLogging;

namespace VeloBrawl.V2.VB24BXIM9CA.GameProxy.GameMessagingPart;

public class Messaging(LaserTcpSession laserTcpSession) : IMessaging
{
    public bool LoginReceived { get; set; }

    public int NextMessage(byte[] buffer, long offset, long size, out (int, int, int) header)
    {
        if (offset + size > 1536 + 7)
            return (header = (-666, 0, 0)).Item1;

        buffer = buffer.Skip((int)offset).ToArray();
        header = ReadHeader(buffer);

        if (header.Item2 > buffer.Length || header.Item2 > size)
            return (header = (-666, 0, 0)).Item1;

        return ReadNewMessage(buffer.Take(7 + header.Item2).ToArray(), header.Item1, header.Item2, header.Item3);
    }

    public int ReadNewMessage(byte[] buffer, int type, int length, int version)
    {
        switch (type)
        {
            case 10100:
                return -666;
            case 10101:
                LoginReceived = true;
                break;
            case 10108 when LoginReceived:
                laserTcpSession.KeepAlive();
                break;
            default:
                if (!LoginReceived) return -666;
                break;
        }

        var info = DebugInfoCollector.PacketCollectorY[type];
        NodeServerProcessor.SendMessageToNode(info.Item2, laserTcpSession.Id, buffer);

        ConsoleLogger.WriteTextWithPrefix(ConsoleLogger.Prefixes.Tcp, $"New message received: {type}: {info}.");
        return 0;
    }

    public void Send(byte[] buffer)
    {
        laserTcpSession.SendAsync(buffer.Concat(new byte[] { 0xFF, 0xFF, 0x0, 0x0, 0x0, 0x0, 0x0 }).ToArray());
    }

    public int EncryptAndWrite(ref byte[] buffer)
    {
        var header = buffer.Take(7);
        var payload = buffer.Skip(7);

        buffer = header.Concat(payload).ToArray();
        return 0;
    }

    /// <summary>
    ///     Reads message header in tuple version.
    /// </summary>
    /// <param name="headerBuffer">byteArray of received packet.</param>
    /// <returns>(type, length, version)</returns>
    public static (int, int, int) ReadHeader(byte[] headerBuffer)
    {
        var v1 = (headerBuffer[0] << 8) | headerBuffer[1]; // messageType (int16)
        var v2 = (headerBuffer[2] << 16) | (headerBuffer[3] << 8) | headerBuffer[4]; // messageLength (int24)
        var v3 = (headerBuffer[5] << 8) | headerBuffer[6]; // messageVersion (int16)

        return (v1, v2, v3);
    }

    /// <summary>
    ///     Writes message header.
    /// </summary>
    /// <param name="payload">byteArray of message</param>
    /// <param name="t">messageType</param>
    /// <param name="v">messageVersion</param>
    /// <returns></returns>
    public static byte[] WriteHeader(byte[] payload, int t, int v)
    {
        var final = new byte[payload.Length + 7];
        {
            // int16
            final[0] = (byte)(t >> 8);
            final[1] = (byte)t;
            // messageType

            // int24
            final[2] = (byte)(payload.Length >> 16);
            final[3] = (byte)(payload.Length >> 8);
            final[4] = (byte)payload.Length;
            // messageLength

            // int16
            final[5] = (byte)(v >> 8);
            final[6] = (byte)v;
            // messageVersion
        }

        return final;
    }
}