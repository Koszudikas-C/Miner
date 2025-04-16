namespace UpdateClientService.Interface;

public interface IConnectionAndAuth
{ 
    Task ConnectAndAuthAsync(CancellationToken cts = default);
}