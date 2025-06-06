using LibSocketAndSslStreamRemote.Entities.Enum;

namespace LibSocketAndSslStreamRemote.Interface;

public interface ISocketMiring
{
    Task InitializeAsync(uint port, int maxConnection,TypeAuthMode typeAuthMode,
        CancellationToken cts = default);
    Task ReconnectAsync(uint port , int maxConnection, TypeAuthMode typeAuthMode, 
        CancellationToken cts = default);
}
