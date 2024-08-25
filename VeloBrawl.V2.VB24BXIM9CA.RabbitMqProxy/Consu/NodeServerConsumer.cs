using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using VeloBrawl.V2.VB24BXIM9CA.Auxiliary.Settings;

namespace VeloBrawl.V2.VB24BXIM9CA.RabbitMqProxy.Consu;

public class NodeServerConsumer(string node)
{
    public IModel Channel { get; private set; } = null!;

    public void Create()
    {
        var factory = new ConnectionFactory { HostName = AppConfig.RabbitMqHost };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.QueueDeclare(node, true, false, false, null);
        Channel = channel;
    }

    public void ConsumeMessages(EventHandler<BasicDeliverEventArgs> receivedCallback)
    {
        try
        {
            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += receivedCallback;

            Channel.BasicConsume(node, true, consumer);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Thread.Sleep(1500);

            ConsumeMessages(receivedCallback);
        }
    }
}