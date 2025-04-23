using System.Net.Sockets;
using LibRemoteAndClient.Enum;
using LibSocket.Entities.Enum;

namespace LibSocket.Interface;

public interface ISocketMiring
{
    Task InitializeAsync(uint port, int maxConnection,
        TypeRemoteClient typeEnum, TypeAuthMode typeAuthMode,
        CancellationToken cts = default);
}