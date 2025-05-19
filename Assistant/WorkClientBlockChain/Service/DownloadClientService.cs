using LibDownload.Interface;
using LibDto.Dto;
using LibReceive.Interface;
using LibRemoteAndClient.Enum;
using LibSend.Interface;
using WorkClientBlockChain.Connection.Interface;

namespace WorkClientBlockChain.Service;

public class DownloadService(
    ISend<DownloadRequestDto> sendDownloadRequest,
    IClientConnected clientConnected,
    ILogger<DownloadService> logger, IReceive receive) : IDownload
{
    public async Task DownloadAsync(object obj, CancellationToken cts = default)
    {
        if (obj is not DownloadRequestDto downloadRequestDto)
        {
            logger.LogInformation("obj is not expected type");
            throw new InvalidCastException("Obj is not expected type");
        }

        try
        {
            var clientInfo = clientConnected.GetClientInfo()!;
            downloadRequestDto.ClientInfoId = clientInfo.Id;
            
            await sendDownloadRequest.SendAsync(downloadRequestDto, clientInfo,
                TypeSocketSsl.SslStream, cts);
            logger.LogInformation($"Obj sent to the server successfully!. Type: {downloadRequestDto.FileAvailable}");

            await receive.ReceiveDataFileAsync(clientInfo, TypeSocketSsl.SslStream, 1, cts);
        }
        catch (Exception e)
        {
            logger.LogError($"Error sending the download parameters. Error{e.Message}");
            throw new Exception($"Error sending the download parameters. Error{e.Message}");
        }
    }
}