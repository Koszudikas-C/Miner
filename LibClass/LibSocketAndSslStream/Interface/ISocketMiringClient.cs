using LibSocketAndSslStream.Entities.Enum;

namespace LibSocketAndSslStream.Interface;

public interface ISocketMiringClient
{
    Task InitializeAsync(uint port, int maxConnection,TypeAuthMode typeAuthMode,
        CancellationToken cts = default);
    Task ReconnectAsync(uint por , int maxConnection, TypeAuthMode typeAuthMode, 
        CancellationToken cts = default);
}