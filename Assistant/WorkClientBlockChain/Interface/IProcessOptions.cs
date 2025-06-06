namespace WorkClientBlockChain.Interface;

public interface IProcessOptions
{
    Task ProcessAsync(object obj, CancellationToken cts = default);
}
