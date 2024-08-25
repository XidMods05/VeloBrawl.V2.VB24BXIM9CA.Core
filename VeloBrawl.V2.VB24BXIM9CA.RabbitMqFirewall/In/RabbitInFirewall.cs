using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using VeloBrawl.V2.VB24BXIM9CA.Auxiliary.Settings;
using VeloBrawl.V2.VB24BXIM9CA.RabbitMqFirewall.Lexical;

namespace VeloBrawl.V2.VB24BXIM9CA.RabbitMqFirewall.In;

public class RabbitInFirewall
{
    public IModel Channel { get; private set; } = null!;

    public void Create()
    {
        var factory = new ConnectionFactory { HostName = AppConfig.RabbitMqHost };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.QueueDeclare("VeloBrawl.i.firewall", true, false, false, null);
        Channel = channel;

        ConsumeMessages();
    }

    private void ConsumeMessages()
    {
        try
        {
            var consumer = new EventingBasicConsumer(Channel);
            {
                consumer.Received += (_, args) =>
                    LexicalAnalyzer.AnalyzeAndProcess(Encoding.UTF8.GetString(args.Body.ToArray()));
            }

            Channel.BasicConsume("VeloBrawl.i.firewall", true, consumer);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Thread.Sleep(1000);

            ConsumeMessages();
        }
    }
}