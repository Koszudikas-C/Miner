using System.Net.Sockets;

namespace LibEntitiesRemote.Interface;

public interface ISocketWrapper
{
    bool Connected { get; }
    string RemoteEndPoint { get; }
    string LocalEndPoint { get; }
    int PortRemote { get; }
    Socket InnerSocket { get; }
}
