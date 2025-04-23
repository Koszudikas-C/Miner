namespace WorkClientBlockChain.Middleware.Interface;

public interface IConnectionMiddleware
{
    Task MonitoringConnectionWorkAsync(CancellationToken cts = default);
}
