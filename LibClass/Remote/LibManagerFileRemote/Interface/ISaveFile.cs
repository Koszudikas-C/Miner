using LibManagerFileRemote.Entities;

namespace LibManagerFileRemote.Interface;

public interface ISaveFile
{
    Task<string> SaveFileWriteAsync(ConfigSaveFile configSaveFile,
        CancellationToken cts = default);
    Task<string> SaveFileWriteBytesAsync(ConfigSaveFile configSaveFile,
        CancellationToken cts = default);
    string SaveFileWrite(ConfigSaveFile configSaveFile);
    string SaveFileWriteBytes(ConfigSaveFile configSaveFile);
}
