namespace LibDownload.Interface;

public interface IDownload
{
    Task DownloadAsync(object obj, CancellationToken cts = default);
}