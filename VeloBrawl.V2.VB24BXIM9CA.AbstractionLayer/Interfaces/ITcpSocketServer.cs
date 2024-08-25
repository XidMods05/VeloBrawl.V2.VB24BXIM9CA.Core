namespace VeloBrawl.V2.VB24BXIM9CA.AbstractionLayer.Interfaces;

public interface ITcpSocketServer
{
    public int NumberOfAllConnections { get; set; }
    public int NumberOfActiveConnections { get; set; }
}