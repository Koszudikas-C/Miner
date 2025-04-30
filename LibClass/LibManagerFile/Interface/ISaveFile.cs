using LibManagerFile.Entities;

namespace LibManagerFile.Interface;

public interface ISaveFile
{
    Task<bool> SaveFileWriteAsync(ConfigSaveFile configSaveFile,
        CancellationToken cts = default);
    Task<bool> SaveFileWriteByteAsync(ConfigSaveFile configSaveFile,
        CancellationToken cts = default);
    bool SaveFileWrite(ConfigSaveFile configSaveFile);
    bool SaveFileByteWrite(ConfigSaveFile configSaveFile);
}