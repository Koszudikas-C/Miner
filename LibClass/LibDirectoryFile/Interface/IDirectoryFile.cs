namespace LibDirectoryFile.Interface;

public interface IDirectoryFile
{
    Task<string> GetDirectoryDefaultTorrAsync();
    string GetFileNameDefaultTorr();

    string GetDirectoryDefaultTorr();
}