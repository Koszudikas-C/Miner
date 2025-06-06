
namespace ApiRemoteWorkClientBlockChain.Interface;

public interface IProcessOptions
{
    Task ProcessAsync(CancellationToken cts = default);
}
