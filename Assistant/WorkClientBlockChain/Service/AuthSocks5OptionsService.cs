using LibClassGetProcessInfo.Entities;
using LibClassGetProcessInfo.Interface;
using LibClassManagerOptions.Entities;
using LibClassManagerOptions.Entities.Enum;
using LibClassProcessOperations.Entities;
using LibClassProcessOperations.Interface;
using LibDownload.Interface;
using LibDto.Dto;
using LibDto.Dto.Enum;
using LibHandler.EventBus;
using LibManagerFile.Entities.Enum;
using LibMapperObj.Interface;
using LibProcess.Interface;
using LibRemoteAndClient.Entities.Remote;
using WorkClientBlockChain.Connection.Interface;

namespace WorkClientBlockChain.Service;

internal class AuthSocks5OptionsService(
    ILogger<AuthSocks5OptionsService> logger,
    IClientConnected clientConnected,
    IGetProcessInfo getProcessInfo,
    IDownload download,
    IMapperObj mapperObj,
    IProcessKill processKill)
    : IProcessOptionsClient
{
    private readonly ILogger<AuthSocks5OptionsService> _logger = logger;
    private readonly IClientConnected _clientConnected = clientConnected;
    private readonly IGetProcessInfo _getProcessInfo = getProcessInfo;
    private readonly IDownload _download = download;
    private readonly IMapperObj _mapperObj = mapperObj;
    private readonly IProcessKill _processKill = processKill;
    private readonly GlobalEventBusClient _globalEventBusClient = GlobalEventBusClient.Instance!;

    /// <summary>
    /// Process method is always default in a matter of parameters. Summarizing automated.
    /// </summary>
    public async Task ProcessAsync(object obj, CancellationToken cts = default)
    {
        if (obj is not ParamsSocks5)
            _globalEventBusClient.Publish(new ParamsManagerOptionsResponse()
            {
                TypeName = "Empty",
                TypeManagerOptionsResponse = TypeManagerOptionsResponse.TypeNotDefined
            });

        var paramsSocks5 = obj as ParamsSocks5;

        await VerifyProcessRunningAsync(paramsSocks5!.ParamsGetProcessInfo, cts);
    }

    private async Task VerifyProcessRunningAsync(ParamsGetProcessInfo paramsGetProcessInfo,
        CancellationToken cts = default)
    {
        ProcessInfo? processInfo = null;
        try
        {
            if (!string.IsNullOrWhiteSpace(paramsGetProcessInfo.NameProcess))
            {
                var resultList = _getProcessInfo.GetProcessInfo(paramsGetProcessInfo.NameProcess);
                processInfo = resultList.FirstOrDefault();
            }
            else if (paramsGetProcessInfo.Port > 0)
            {
                processInfo = _getProcessInfo.GetProcessInfo(paramsGetProcessInfo.Port);
            }

            if (processInfo != null && processInfo.Pid > 0)
            {
                FinalizeProcess(processInfo);
            }

            await DownloadTorrcAsync(cts);
        }
        catch (Exception e)
        {
            _logger.LogError($"Error when checking and finishing the process. Error:{e.Message}");
            processInfo!.LastError = e.Message;
            _globalEventBusClient.Publish(new ParamsManagerOptionsResponseDto
            {
                TypeManagerOptionsResponse = TypeManagerOptionsResponseDto.ProcessNotKill,
                ParamsForProcessResponse = processInfo
            });

            throw new Exception($"Error when checking and finishing the process. Error:{e.Message}");
        }
    }

    private void FinalizeProcess(ProcessInfo processInfo)
    {
        if (processInfo.Pid == 0)
            return;

        _processKill.ProcessKill(processInfo);
    }

    private async Task DownloadTorrcAsync(CancellationToken cts = default)
    {
        try
        {
            var downloadRequest = new DownloadRequest(TypeAvailableFile.Tor, TypeExtensionFile.Zip);
            var downloadRequestDto = _mapperObj.Map<DownloadRequest, DownloadRequestDto>(downloadRequest);

            var taskHeaderDto = new TaskCompletionSource<UploadResponseHeaderDto>();
            var taskCompletionSource = new TaskCompletionSource<UploadResponseDto>();

            void HandlerHeader(UploadResponseHeaderDto uploadResponseHeaderDto)
            {
                taskHeaderDto.TrySetResult(uploadResponseHeaderDto);
                _logger.LogInformation($"Obj Header from the file was successfully received! " +
                                       $". {uploadResponseHeaderDto.NameFile}");
            }
            
            void Handler(UploadResponseDto uploadResponseDto)
            {
                taskCompletionSource.TrySetResult(uploadResponseDto);
                _globalEventBusClient.Unsubscribe<UploadResponseDto>(Handler);
            }

            _globalEventBusClient.Subscribe<UploadResponseHeaderDto>(HandlerHeader);
            _globalEventBusClient.Subscribe<UploadResponseDto>(Handler);

            await _download.DownloadAsync(downloadRequestDto, cts);

            var uploadResponse = await taskCompletionSource.Task;

            VerifyInstalledTorrc(uploadResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error when downloading the Torrc. Error: {ex.Message}");
            _globalEventBusClient.Publish(new ParamsManagerOptionsResponseDto()
            {
                TypeManagerOptionsResponse = TypeManagerOptionsResponseDto.NotFound,
                ParamsForProcessResponse = _logger
            });
            throw new Exception("Error when downloading the Torrc.");
        }
    }

    private void VerifyInstalledTorrc(UploadResponseDto uploadResponseDto)
    {
        _logger.LogInformation($"Gross file data was successfully received!. {uploadResponseDto}");
    }
}