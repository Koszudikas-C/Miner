using LibSocketAndSslStream.Entities.Enum;

namespace LibSocketAndSslStream.Interface;

public interface ISocketMiring
{
    Task InitializeAsync(uint port, int maxConnection,
        TypeRemoteClient typeEnum, TypeAuthMode typeAuthMode,
        CancellationToken cts = default);
    Task ReconnectAsync(uint por , int maxConnection, TypeRemoteClient typeRemoteCient,
        TypeAuthMode typeAuthMode, CancellationToken cts = default);
}