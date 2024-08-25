namespace VeloBrawl.V2.VB24BXIM9CA.AbstractionLayer.Interfaces;

public interface IMessaging
{
    public bool LoginReceived { get; set; }

    public int NextMessage(byte[] buffer, long offset, long size, out (int, int, int) header);
    public int ReadNewMessage(byte[] buffer, int type, int length, int version);
    public void Send(byte[] buffer);
    public int EncryptAndWrite(ref byte[] buffer);
}