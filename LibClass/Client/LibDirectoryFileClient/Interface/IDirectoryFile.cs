namespace LibDirectoryFileClient.Interface;

public interface IDirectoryFile
{
    Task<string> GetDirectoryDefaultTorrAsync();
    string GetFileNameDefaultTorr();

    string GetDirectoryDefaultTorr();
}
