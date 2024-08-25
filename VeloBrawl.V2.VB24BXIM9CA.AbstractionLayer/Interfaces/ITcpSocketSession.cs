using Veltonsoft.BrawlStars.Velofauna.VelofaunaProjects.BaseNetworking.Slave;

namespace VeloBrawl.V2.VB24BXIM9CA.AbstractionLayer.Interfaces;

public interface ITcpSocketSession
{
    public TcpSession Session { get; set; }
    public IMessaging Messaging { get; set; }
}