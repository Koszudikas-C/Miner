namespace WorkClientBlockChain.Utils.Interface;

public interface IPortOpen
{
    Task<bool> IsOpenPortAsync(int port, CancellationToken cts = default);
}

