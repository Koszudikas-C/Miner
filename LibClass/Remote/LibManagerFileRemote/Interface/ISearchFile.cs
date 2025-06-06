using LibManagerFileRemote.Entities.Enum;

namespace LibManagerFileRemote.Interface;

public interface ISearchFile
{
    Task<object> SearchFileAsync(TypeFile typeFile, CancellationToken cts = default);
    
    object SearchFile(TypeFile typeFile);
}
