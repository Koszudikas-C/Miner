using LibClassManagerOptions.Entities.Enum;
using LibClassManagerOptions.Interface;
using LibClassProcessOperations.Interface;
using LibHandler.EventBus;
using LibRemoteAndClient.Enum;
using LibSend.Interface;
using WorkClientBlockChain.Connection.Interface;
using WorkClientBlockChain.Utils;
using WorkClientBlockChain.Utils.Interface;

namespace WorkClientBlockChain.Service;

public class ManagerOptionsService : IManagerOptions
{
    private readonly IProcessOptions _processOptions;
    private readonly ILogger<ManagerOptionsService> _logger;
    private readonly ISend<TypeManagerResponseOperations> _send;
    private readonly IClientContext _clientContext;
    private readonly IPortOpen _portOpen;
    private readonly IConnectionValidation _connectionValidation;
    private readonly GlobalEventBusClient _globalEventBusClient = GlobalEventBusClient.Instance!;
    private CancellationTokenSource _ctsSource = new();

    public ManagerOptionsService(IProcessOptions processOptions, ILogger<ManagerOptionsService> logger
    , ISend<TypeManagerResponseOperations> send, IClientContext clientContext,
        IPortOpen portOpen, IConnectionValidation connectionValidation)
    {
        _processOptions = processOptions;
        _logger = logger;
        _send = send;
        _clientContext = clientContext;
        _portOpen = portOpen;
        _connectionValidation = connectionValidation;

        _globalEventBusClient.Subscribe<TypeManagerOptions>(async void
            (handler) => await InitializeOptions(handler));
        _globalEventBusClient.Subscribe<TypeManagerResponseOperations>(async void
            (handler) => await ResponseOptions(handler));
    }

    public async Task InitializeOptions(TypeManagerOptions expression,
        CancellationToken cts = default)
    {

        switch (expression)
        {
            case TypeManagerOptions.AuthSocks5:
                await _processOptions.IsProcessAuthSocks5Async(_ctsSource.Token);
                break;
            case TypeManagerOptions.CheckAppClientBlockChain:
                await _processOptions.IsProcessCheckAppClientBlockChain(_ctsSource.Token);
                break;
            case TypeManagerOptions.DownloadAppClientBlockChain:
                await _processOptions.IsProcessDownloadAppClientBlockChain(_ctsSource.Token);
                break;
            case TypeManagerOptions.Logs:
                await _processOptions.IsProcessLogs(_ctsSource.Token);
                break;
            case TypeManagerOptions.StatusConnection:
                await _processOptions.IsProcessStatusConnection(_ctsSource.Token);
                break;
            case TypeManagerOptions.StatusTransaction:
                await _processOptions.IsProcessStatusTransaction(_ctsSource.Token);
                break;
            case TypeManagerOptions.CancelOperations:
                ResetCancellationToken();
                break;
            case TypeManagerOptions.Error:
            default:
                ResetCancellationToken();
                _logger.LogError($"Check the command arguments is invalid: {expression}");
                throw new ArgumentException($"Check the command arguments is invalid: {expression}");
        }
    }

    public async Task ResponseOptions(TypeManagerResponseOperations expression,
        CancellationToken cts = default)
    {
        switch (expression)
        {
            case TypeManagerResponseOperations.Success:
                await Send(expression);
                _logger.LogInformation($"Success");
                break;
            case TypeManagerResponseOperations.NotFound:
                await Send(expression);
                _logger.LogInformation($"NotFound");
                break;
            case TypeManagerResponseOperations.Unauthorized:
                await Send(expression);
                _logger.LogInformation($"Unauthorized");
                break;
            case TypeManagerResponseOperations.Error:
                await Send(expression);
                break;
            case TypeManagerResponseOperations.InvalidRequest:
                await Send(expression);
                break;
            case TypeManagerResponseOperations.Timeout:
                await Send(expression);
                break;
            case TypeManagerResponseOperations.AlreadyExists:
                await Send(expression);
                break;
            case TypeManagerResponseOperations.ValidationFailed:
                await Send(expression);
                break;
            case TypeManagerResponseOperations.PartialSuccess:
                await Send(expression);
                break;
            case TypeManagerResponseOperations.Pending:
                await Send(expression);
                break;
            default:
                ResetCancellationToken();
                _logger.LogError($"Check the command arguments is invalid: {expression}");
                await Send(expression);
                throw new ArgumentException($"Check the command arguments is invalid: {expression}");
        }
    }

    private async Task Send(TypeManagerResponseOperations expression)
    {
        var clientInfo = _clientContext.GetClientInfo();
        var typeSocketSsl = TypeSocketSsl.Socket;
        if (clientInfo is not null && clientInfo.SslStreamWrapper!.IsAuthenticated!)
        {
            typeSocketSsl = TypeSocketSsl.SslStream;
        }

        await _send.SendAsync(expression, clientInfo!, typeSocketSsl, _ctsSource.Token);
    }

    private void ResetCancellationToken()
    {
        _ctsSource.Cancel();
        _ctsSource = new CancellationTokenSource();
    }
}