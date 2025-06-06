using LibEntitiesClient.Interface;

namespace LibEntitiesClient.Entities;

public class ObjSocketSslStream
{
    public Guid Id { get; set; }
    public ISocketWrapper? SocketWrapper { get; set; }
    public ISslStreamWrapper? SslStreamWrapper { get; set; }

    public void Disconnect()
    {
        if (SocketWrapper is null) return;

        SocketWrapper.InnerSocket.Close();
        SocketWrapper.InnerSocket.Dispose();
        
        if (SslStreamWrapper is null) return;

        SslStreamWrapper!.InnerSslStream!.Close();
        SslStreamWrapper.InnerSslStream.Dispose();
    }
}