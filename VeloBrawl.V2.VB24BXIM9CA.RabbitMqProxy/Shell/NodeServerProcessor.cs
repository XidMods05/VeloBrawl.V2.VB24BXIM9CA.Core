using System.Collections.Concurrent;
using VeloBrawl.V2.VB24BXIM9CA.AbstractionLayer.Interfaces;
using VeloBrawl.V2.VB24BXIM9CA.RabbitMqProxy.Produ;

namespace VeloBrawl.V2.VB24BXIM9CA.RabbitMqProxy.Shell;

public static class NodeServerProcessor
{
    private static readonly ConcurrentDictionary<Guid, ITcpSocketSession> TcpSocketSessions = new();
    private static readonly ConcurrentDictionary<Guid, byte[]> CachedByteArrayByGuid = new();

    private static readonly ConcurrentDictionary<int, NodeServerProducer> NodeServerProducers = new();

    public static void AddTcpSocketSession(Guid id, ITcpSocketSession session)
    {
        TcpSocketSessions.TryAdd(id, session);
        CachedByteArrayByGuid.TryAdd(id, id.ToByteArray());
    }

    public static ITcpSocketSession? GetTcpSocketSession(Guid id)
    {
        return TcpSocketSessions.GetValueOrDefault(id);
    }

    public static byte[]? GetCachedByteArray(Guid id)
    {
        return CachedByteArrayByGuid.GetValueOrDefault(id);
    }

    public static void RemoveTcpSocketSession(Guid id)
    {
        TcpSocketSessions.TryRemove(id, out _);
        CachedByteArrayByGuid.TryRemove(id, out _);
    }

    public static void AddNodeServerProducer(int node, NodeServerProducer producer)
    {
        NodeServerProducers.TryAdd(node, producer);
    }

    public static NodeServerProducer? GetNodeServerProducer(int node)
    {
        return NodeServerProducers.GetValueOrDefault(node);
    }

    public static void RemoveNodeServerProducer(int node)
    {
        NodeServerProducers.TryRemove(node, out _);
    }

    public static void SendMessageToNode(int node, Guid id, byte[] message)
    {
        var byteArray = GetCachedByteArray(id);
        if (byteArray == null) return;

        var intBuffer = new byte[4];
        {
            intBuffer[0] = (byte)(byteArray.Length >> 24);
            intBuffer[1] = (byte)(byteArray.Length >> 16);
            intBuffer[2] = (byte)(byteArray.Length >> 8);
            intBuffer[3] = (byte)byteArray.Length;
        }

        NodeServerProducers[node].SendMessage(intBuffer.Concat(byteArray).Concat(message).ToArray());
    }

    public static void ReceiveMessageFromNode(byte[] message)
    {
        var len = (message[0] << 24) | (message[1] << 16) | (message[2] << 8) | message[3];
        var id = new Guid(message.Skip(4).Take(len).ToArray());

        GetTcpSocketSession(id)?.Messaging.Send(message.Skip(4 + len).ToArray());
    }
}