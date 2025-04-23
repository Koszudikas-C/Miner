using LibClassManagerOptions.Entities.Enum;

namespace WorkClientBlockChain.Interface;

public interface IDownload
{
    Task ManagerAsync(TypeDownLoad type, CancellationToken cts = default);
}
