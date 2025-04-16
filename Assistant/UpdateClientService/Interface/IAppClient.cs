
namespace UpdateClientService.Interface;

public interface IAppClient
{
    Task InstallAppAsync(CancellationToken cts);
    Task UpdateAppAsync(CancellationToken cts);
    Task RemoveAppAsync(CancellationToken cts);
    Task IsProcessRunningAsync(CancellationToken cts);
}
