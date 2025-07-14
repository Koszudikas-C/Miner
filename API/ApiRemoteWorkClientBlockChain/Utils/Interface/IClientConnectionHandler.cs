using LibSocketAndSslStreamRemote.Entities;

namespace ApiRemoteWorkClientBlockChain.Utils.Interface;

public interface IClientConnectionHandler
{
    Task OnReceiveSocketConnectedEvent(SocketsConnectedEvent socketsConnectedEvent,
        CancellationToken cts = default);
}