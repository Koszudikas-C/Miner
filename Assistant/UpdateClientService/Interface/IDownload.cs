
using UpdateClientService.Enum;

namespace UpdateClientService.Interface;

public interface IDownload
{
    Task ManagerAsync(TypeDownLoad type, CancellationToken cts = default);
}
