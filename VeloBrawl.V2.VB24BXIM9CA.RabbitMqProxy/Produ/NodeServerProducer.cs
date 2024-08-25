using RabbitMQ.Client;
using VeloBrawl.V2.VB24BXIM9CA.Auxiliary.Settings;

namespace VeloBrawl.V2.VB24BXIM9CA.RabbitMqProxy.Produ;

public class NodeServerProducer(string node)
{
    private readonly object _providerLock = new();

    public IModel Channel { get; private set; } = null!;

    public void Create()
    {
        var factory = new ConnectionFactory { HostName = AppConfig.RabbitMqHost };
        var connection = factory.CreateConnection();

        Channel = connection.CreateModel();
    }

    public void SendMessage(byte[] message)
    {
        lock (_providerLock)
        {
            Channel.BasicPublish("", node, null, message);
        }
    }
}