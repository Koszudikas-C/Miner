using LibManagerFileClient.Entities.Enum;

namespace LibManagerFileClient.Interface;

public interface ISearchFile
{
    Task<object> SearchFileAsync(TypeFile typeFile, CancellationToken cts = default);
    
    object SearchFile(TypeFile typeFile);
}
