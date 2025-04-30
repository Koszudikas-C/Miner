namespace WorkClientBlockChain.Utils.Interface;

public interface IDownloadAll
{
    Task<bool> DownloadAsync(string url, string path, 
        CancellationToken cts = default);
    
}