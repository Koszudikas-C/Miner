using LibDto.Dto;

namespace LibUpload.Interface;

public interface IUploadSend<in T, in TW>
{
    Task<bool> UploadSendAsync(T obj, TW dataDto, CancellationToken cts = default);
}