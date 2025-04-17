using LibHandler.EventBus;
using LibHandler.Interface;
using WorkClientBlockChain.Entities.Enum;
using WorkClientBlockChain.Interface;

namespace WorkClientBlockChain.Service;

public class ManagerOptionsService : IManagerOptions
{
    private readonly IProcessOptions _processOptions;
    private readonly ILogger<ManagerOptionsService> _logger;
    private readonly GlobalEventBusClient _globalEventBusClient = GlobalEventBusClient.Instance!;
    private CancellationTokenSource _ctsSource = new();
    
    public ManagerOptionsService(IProcessOptions processOptions, ILogger<ManagerOptionsService> logger)
    {
        _processOptions = processOptions;
        _logger = logger;
        _globalEventBusClient.Subscribe<TypeManagerOptions>(InitializeOptions);
    }
    public void InitializeOptions(TypeManagerOptions expression)
    {
        switch (expression)
        {
            case TypeManagerOptions.Auth:
                _processOptions.IsProcessAuthSocks5(_ctsSource.Token);
                break;
            case TypeManagerOptions.CheckAppClientBlockChain:
                _processOptions.IsProcessCheckAppClientBlockChain(_ctsSource.Token);
                break;
            case TypeManagerOptions.DownloadAppClientBlockChain:
                _processOptions.IsProcessDownloadAppClientBlockChain(_ctsSource.Token);
                break;
            case TypeManagerOptions.Logs:
                _processOptions.IsProcessLogs(_ctsSource.Token);
                break;
            case TypeManagerOptions.StatusConnection:
                _processOptions.IsProcessStatusConnection(_ctsSource.Token);
                break;
            case TypeManagerOptions.StatusTransaction:
                _processOptions.IsProcessStatusTransaction(_ctsSource.Token);
                break;
            case TypeManagerOptions.CancelOperations:
                ResetCancellationToken();
                break;
            default:
                ResetCancellationToken();
                _logger.LogError($"Check the command arguments is invalid: {expression}");
                throw new ArgumentException($"Check the command arguments is invalid: {expression}");
        }
    }

    private void ResetCancellationToken()
    {
        _ctsSource.Cancel();
        _ctsSource = new CancellationTokenSource();
    }
}