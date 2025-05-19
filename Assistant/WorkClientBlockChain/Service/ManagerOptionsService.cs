using System.Text.Json;
using LibClassManagerOptions.Entities;
using LibClassManagerOptions.Entities.Enum;
using LibClassManagerOptions.Interface;
using LibClassProcessOperations.Entities;
using LibClassProcessOperations.Interface;
using LibCryptography.Entities;
using LibCryptography.Interface;
using LibDto.Dto;
using LibDto.Dto.Enum;
using LibHandler.EventBus;
using LibManagerFile.Entities.Enum;
using LibManagerFile.Interface;
using LibMapperObj.Interface;
using LibRemoteAndClient.Enum;
using LibSend.Interface;
using LibSocketAndSslStream.Entities;
using WorkClientBlockChain.Connection.Interface;

namespace WorkClientBlockChain.Service;

public class ManagerOptionsService<T> : IManagerOptions<T>
{
    private readonly ILogger<ManagerOptionsService<T>> _logger;
    private readonly ISend<ParamsManagerOptionsResponseDto> _send;
    private readonly IClientConnected _clientConnected;
    private readonly ICryptographFile _cryptographFile;
    private readonly ISearchFile _searchFile;
    private readonly IProcessOptionsClient _processOptionsClient;
    private readonly GlobalEventBusClient _globalEventBusClient = GlobalEventBusClient.Instance!;
    private readonly IMapperObj _mapperObj;
    private CancellationTokenSource _ctsSource = new();
    private readonly string _pathFile = Path.Combine(Directory.GetCurrentDirectory(), "Resources" + "koewa.json");

    public ManagerOptionsService(ILogger<ManagerOptionsService<T>> logger,
        ISend<ParamsManagerOptionsResponseDto> send, IClientConnected clientConnected,
        ICryptographFile cryptographFile, ISearchFile searchFile,
        IProcessOptionsClient processOptionsClient, IMapperObj mapperObj)
    {
        _logger = logger;
        _send = send;
        _clientConnected = clientConnected;
        _cryptographFile = cryptographFile;
        _searchFile = searchFile;
        _processOptionsClient = processOptionsClient;
        _mapperObj = mapperObj;

        ManagerSubscribeType();
    }

    private void ManagerSubscribeType()
    {
        _globalEventBusClient.Subscribe<ParamsManagerOptionsDto<ParamsSocks5Dto>>(
            (handler) => _ = OnReceiveParamsOptionsSocks5Async(handler));
        
        //Response
        _globalEventBusClient.Subscribe<ParamsManagerOptionsResponseDto>(
            (handler) => _ = ResponseOptionsAsync(handler));
    }

    public async Task InitializeOptionsAsync(ParamsManagerOptions<T> paramsManagerOptions,
        CancellationToken cts = default)
    {
        try
        {
            switch (paramsManagerOptions.TypeManagerOptions)
            {
                case TypeManagerOptions.AuthSocks5:
                    var paramsSocks5 = paramsManagerOptions.GetParamsForProcess<ParamsSocks5>();
                    await _processOptionsClient.ProcessAsync(paramsSocks5, cts);
                    break;
                case TypeManagerOptions.CheckAppClientBlockChain:
                    break;
                case TypeManagerOptions.DownloadAppClientBlockChain:
                    break;
                case TypeManagerOptions.Logs:
                    break;
                case TypeManagerOptions.StatusConnection:
                    break;
                case TypeManagerOptions.StatusTransaction:
                    break;
                case TypeManagerOptions.CancelOperations:
                    ResetCancellationToken();
                    break;
                case TypeManagerOptions.Error:
                default:
                    ResetCancellationToken();
                    _logger.LogError(
                        $"Check the command arguments is invalid: {paramsManagerOptions.TypeManagerOptions}");
                    throw new ArgumentException(
                        $"Check the command arguments is invalid: {paramsManagerOptions.TypeManagerOptions}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task ResponseOptionsAsync(ParamsManagerOptionsResponseDto paramsManagerOptions,
        CancellationToken cts = default)
    {
        switch (paramsManagerOptions.TypeManagerOptionsResponse)
        {
            case TypeManagerOptionsResponseDto.Success:
                _logger.LogInformation($"Success");
                await SendParamsOptionsResponse(paramsManagerOptions, cts);
                break;
            case TypeManagerOptionsResponseDto.NotFound:
                _logger.LogInformation($"NotFound");
                await SendParamsOptionsResponse(paramsManagerOptions, cts);
                break;
            case TypeManagerOptionsResponseDto.Unauthorized:
                _logger.LogInformation($"Unauthorized");
                await SendParamsOptionsResponse(paramsManagerOptions, cts);
                break;
            case TypeManagerOptionsResponseDto.Error:
                break;
            case TypeManagerOptionsResponseDto.InvalidRequest:

                break;
            case TypeManagerOptionsResponseDto.Timeout:

                break;
            case TypeManagerOptionsResponseDto.AlreadyExists:

                break;
            case TypeManagerOptionsResponseDto.ValidationFailed:

                break;
            case TypeManagerOptionsResponseDto.PartialSuccess:

                break;
            case TypeManagerOptionsResponseDto.Pending:

                break;
            case TypeManagerOptionsResponseDto.ProcessNotKill:
                await _send.SendAsync(paramsManagerOptions, _clientConnected.GetClientInfo()!, TypeSocketSsl.SslStream,
                    cts);
                _logger.LogInformation($"Send process not kill");
                break;
            case TypeManagerOptionsResponseDto.SslStreamNotAuthenticated:
            case TypeManagerOptionsResponseDto.SocketNotConnected:
            case TypeManagerOptionsResponseDto.PortNotOpen:
            case TypeManagerOptionsResponseDto.TypeNotDefined:
            default:
                ResetCancellationToken();
                _logger.LogError(
                    $"Check the command arguments is invalid: {paramsManagerOptions.TypeManagerOptionsResponse}");

                throw new ArgumentException(
                    $"Check the command arguments is invalid: {paramsManagerOptions.TypeManagerOptionsResponse}");
        }
    }

    private async Task SendParamsOptionsResponse(ParamsManagerOptionsResponseDto paramsManagerOptions,
        CancellationToken cts = default)
    {
        var clientInfo = _clientConnected.GetClientInfo();
        await _send.SendAsync(paramsManagerOptions, clientInfo!, TypeSocketSsl.SslStream, cts);
    }

    private void ResetCancellationToken()
    {
        _ctsSource.Cancel();
        _ctsSource = new CancellationTokenSource();
    }

    private ConfigVariable GetConfigVariable()
    {
        var data = _searchFile.SearchFile(TypeFile.ConfigVariable);

        var configCryptograph = new ConfigCryptograph(_pathFile);

        switch (data)
        {
            case ConfigVariable config:
                configCryptograph.SetData(config);
                break;
            case byte[] bytes:
                configCryptograph.SetDataBytes(bytes);
                break;
            default:
                throw new FileNotFoundException(nameof(data), "Data not found.");
        }

        var result = _cryptographFile.LoadFile(configCryptograph);

        var obj = JsonSerializer.Deserialize<ConfigVariable>(result);

        return obj!;
    }

    private async Task OnReceiveParamsOptionsSocks5Async(ParamsManagerOptionsDto<ParamsSocks5Dto> paramsManagerOptionsDto)
    {
        if (paramsManagerOptionsDto.ParamsForProcess is null)
        {
            _logger.LogWarning($"The parameter to process cannot be null.{paramsManagerOptionsDto.ParamsForProcess}");
            throw new ArgumentNullException(nameof(paramsManagerOptionsDto.ParamsForProcess));
        }

        var paramsSocks5 = _mapperObj.Map<ParamsSocks5Dto, ParamsSocks5>(paramsManagerOptionsDto.ParamsForProcess);
        await _processOptionsClient.ProcessAsync(paramsSocks5);
    }
}