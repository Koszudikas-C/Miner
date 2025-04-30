using LibSocketAndSslStream.Entities;

namespace WorkClientBlockChain.Interface;

public interface IConnectionAndAuth
{ 
    Task ConnectAndAuthAsync(CancellationToken cts = default);
}