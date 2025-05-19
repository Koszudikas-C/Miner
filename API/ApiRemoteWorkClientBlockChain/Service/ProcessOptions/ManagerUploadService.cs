using ApiRemoteWorkClientBlockChain.Entities.Interface;
using LibDirectoryFile.Interface;
using LibDto.Dto;
using LibDto.Dto.Enum;
using LibHandler.EventBus;
using LibManagerFile.Entities.Enum;
using LibMapperObj.Interface;
using LibRemoteAndClient.Entities;
using LibRemoteAndClient.Entities.Remote;
using LibRemoteAndClient.Entities.Remote.Client;
using LibUpload;
using LibUpload.Interface;
using NuGet.ProjectModel;

namespace ApiRemoteWorkClientBlockChain.Service.ProcessOptions;

public class ManagerUploadService : IManagerUpload
{
    private static bool _subscribed;

    private readonly IMapperObj _mapperObj;
    private readonly ILogger<ManagerUploadService> _logger;
    private readonly IUpload _upload;
    private readonly IUploadSend<ClientInfo, UploadResponseHeaderDto> _uploadSendHeader;
    private readonly IUploadSend<ClientInfo, UploadResponseDto> _uploadSend;
    private readonly IClientConnected _clientConnected;
    private readonly IDirectoryFile _directoryFile;

    private readonly GlobalEventBusRemote _globalEventBusRemote = GlobalEventBusRemote.Instance!;

    public ManagerUploadService(
        IMapperObj mapperObj,
        ILogger<ManagerUploadService> logger,
        IUpload upload,
        IUploadSend<ClientInfo, UploadResponseHeaderDto> uploadSendHeader,
        IUploadSend<ClientInfo, UploadResponseDto> uploadSend,
        IClientConnected clientConnected,
        IDirectoryFile directoryFile)
    {
        _mapperObj = mapperObj;
        _logger = logger;
        _upload = upload;
        _uploadSendHeader = uploadSendHeader;
        _uploadSend = uploadSend;
        _clientConnected = clientConnected;
        _directoryFile = directoryFile;

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        if (_subscribed) return;

        _globalEventBusRemote.Subscribe<DownloadRequestDto>(handle =>
            _ = OnReceiveDownloadRequest(handle));

        _subscribed = true;
    }

    private async Task OnReceiveDownloadRequest(DownloadRequestDto dto)
    {
        try
        {
            var fileAvailable = _mapperObj.Map<TypeAvailableFileDto, TypeAvailableFile>(dto.FileAvailable);
            var typeFile = _mapperObj.Map<TypeExtensionFileDto, TypeExtensionFile>(dto.TypeFile);

            var downloadRequest = _mapperObj.Map(dto, () => new DownloadRequest(fileAvailable, typeFile));

            switch (downloadRequest.FileAvailable)
            {
                case TypeAvailableFile.Tor:
                    var pathFile = PreparationFile(downloadRequest);
                    var updateResponseHeaderDto = PreparationResponseHeader(pathFile);
                    await SendFileHeaderAsync(downloadRequest, updateResponseHeaderDto.Item1);
                    var updateResponseDto = PreparationResponseDto(updateResponseHeaderDto.Item2);
                    await SendFileAsync(downloadRequest, updateResponseDto);
                    break;
                default:
                    _logger.LogWarning("Unsupported file type received: {Type}", downloadRequest.FileAvailable);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing DownloadRequestDto.");
        }
    }

    private string PreparationFile(DownloadRequest downloadRequest)
    {
        try
        {
            if (downloadRequest.ClientInfoId != Guid.Empty)
                return downloadRequest.TypeFile switch
                {
                    TypeExtensionFile.Zip => _directoryFile.GetDirectoryDefaultTorr(),
                    _ => throw new NotSupportedException($"File extension not supported: {downloadRequest.TypeFile}")
                };
            
            _logger.LogCritical("Client ID is empty. Request ignored.");
            throw new ArgumentException("ClientInfo ID is empty.");
        }
        catch (KeyNotFoundException)
        {
            _logger.LogCritical("Client not found for ID {ClientId}. Upload aborted.", downloadRequest.ClientInfoId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during file preparation.");
            throw;
        }
    }

    private (UploadResponseHeaderDto, InfoFile) PreparationResponseHeader(string pathFile)
    {
        var infoFile = _upload.Upload(pathFile);
          
        var uploadResponseHeader = new UploadResponseHeader(
            _directoryFile.GetFileNameDefaultTorr(), infoFile.LengthFile!.Length);

        return (_mapperObj.Map<UploadResponseHeader, UploadResponseHeaderDto>(uploadResponseHeader), infoFile);
    }

    private UploadResponseDto PreparationResponseDto(InfoFile infoFile)
    {
        var uploadResponse = new UploadResponse(
            infoFile.DataByte!);

        return _mapperObj.Map<UploadResponse, UploadResponseDto>(uploadResponse);
    }

    private async Task SendFileHeaderAsync(DownloadRequest downloadRequest,
        UploadResponseHeaderDto uploadResponseHeaderDto)
    {
        try
        {
            var clientInfo = _clientConnected.GetClientInfo(downloadRequest.ClientInfoId);
            var result = await _uploadSendHeader.UploadSendAsync(clientInfo, uploadResponseHeaderDto);

            if (result)
            {
                _logger.LogInformation("File sent successfully. Type: {Type}.{Extension}",
                    downloadRequest.FileAvailable, downloadRequest.TypeFile);
                return;
            }

            _logger.LogWarning("File not sent. Type: {Type}.{Extension}", downloadRequest.FileAvailable,
                downloadRequest.TypeFile);
            throw new FileFormatException("File not send");
        }
        catch (KeyNotFoundException)
        {
            _logger.LogCritical("Client not found for ID {ClientId}. Upload aborted.", downloadRequest.ClientInfoId);
            throw;
        }
    }

    private async Task SendFileAsync(DownloadRequest downloadRequest,
        UploadResponseDto uploadResponseDto)
    {
        try
        {
            var clientInfo = _clientConnected.GetClientInfo(downloadRequest.ClientInfoId);

            var result = await _uploadSend.UploadSendAsync(clientInfo, uploadResponseDto);

            if (result)
            {
                _logger.LogInformation("Arquivo enviado com sucesso!");
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"An error occurred when sending the archive array from the file. Error: {e.Message}");
            throw;
        }
    }
}