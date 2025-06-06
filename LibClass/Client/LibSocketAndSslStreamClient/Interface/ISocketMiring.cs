using LibSocketAndSslStreamClient.Entities.Enum;

namespace LibSocketAndSslStreamClient.Interface;

public interface ISocketMiring
{
    Task InitializeAsync(uint port,TypeAuthMode typeAuthMode,
        CancellationToken cts = default);
    Task ReconnectAsync(uint port, TypeAuthMode typeAuthMode, 
        CancellationToken cts = default);
}
