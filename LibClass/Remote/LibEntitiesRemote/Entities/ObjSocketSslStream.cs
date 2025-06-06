using System.Net.Sockets;
using LibEntitiesRemote.Interface;

namespace LibEntitiesRemote.Entities;

public class ObjSocketSslStream(Socket socket): IAuthDisconnectClient
{
    public Guid Id { get; set; }
    public ISocketWrapper SocketWrapper { get; set; } = new SocketWrapper(socket);
    public ISslStreamWrapper? SslStream { get; set; }

    public bool Connected => socket.Connected;

    public void Disconnect()
    {
      SocketWrapper.InnerSocket.Close();
      
      if(SslStream is null) return;
      
      SslStream!.InnerSslStream!.Close();
    }
}
