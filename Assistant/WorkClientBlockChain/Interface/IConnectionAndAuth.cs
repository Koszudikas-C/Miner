using LibSocket.Entities;

namespace WorkClientBlockChain.Interface;

public interface IConnectionAndAuth
{ 
    Task ConnectAndAuthAsync(ConnectionConfig connectionConfig, CancellationToken cts = default);
}