using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Enum;
using LibSend.Interface;
using LibUpload.Interface;

namespace ApiRemoteWorkClientBlockChain.Service.ProcessOptions;

public class UploadSendService<T, TW>(ILogger<UploadSendService<T, TW>> logger,
    ISend<TW> send) : IUploadSend<T, TW>
{
    public async Task<bool> UploadSendAsync(T obj, TW data,

        CancellationToken cts = default)
    {
        try
        {
            if (obj is not ClientInfo clientInfo)
                throw new InvalidCastException($"It is not possible to use the upload service without the clientInfo type");

            await send.SendFileAsync(data, clientInfo, TypeSocketSsl.SslStream, cts);
            logger.LogInformation($"File has been sent successfully! Data: {data}");
            
            return true;
        }
        catch (Exception e)
        {
            logger.LogError($"It was not possible to send the file to the client. Error: {e.Message}");
            throw new Exception();
        }
    }
}