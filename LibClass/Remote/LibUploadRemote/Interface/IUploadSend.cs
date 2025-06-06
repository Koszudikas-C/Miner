namespace LibUploadRemote.Interface;

public interface IUploadSend<in T, in TW>
{
    Task<bool> UploadSendAsync(T obj, TW dataDto, CancellationToken cts = default);
}
