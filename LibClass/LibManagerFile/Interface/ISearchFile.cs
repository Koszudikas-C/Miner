using LibManagerFile.Entities.Enum;

namespace LibManagerFile.Interface;

public interface ISearchFile
{
    Task<object> SearchFileAsync(TypeFile typeFile, CancellationToken cts = default);
    
    object SearchFile(TypeFile typeFile);
}