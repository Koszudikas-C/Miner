using LibEntitiesClient.Interface;
using LibSocketAndSslStreamClient.Entities;
using LibSocketAndSslStreamClient.Entities.Enum;

namespace LibSocketAndSslStreamClient.Interface;

public interface ISocket
{
    Task InitializeAsync(uint port,TypeAuthMode typeAuthMode,
        CancellationToken cts = default);
    Task ReconnectAsync(ISocketWrapper socketWrapper, TypeAuthMode typeAuthMode, 
        CancellationToken cts = default);

    delegate void SocketEventHandler(object sender, SocketsConnectedArgs e);
}
