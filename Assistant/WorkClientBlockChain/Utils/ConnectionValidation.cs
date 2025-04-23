using LibClassManagerOptions.Entities.Enum;
using LibCommunicationStatus;
using LibHandler.EventBus;
using WorkClientBlockChain.Connection;
using WorkClientBlockChain.Connection.Interface;
using WorkClientBlockChain.FilterException;
using WorkClientBlockChain.Utils.Interface;

namespace WorkClientBlockChain.Utils;

internal class ConnectionValidation(ILogger<ConnectionValidation> logger, 
    IClientContext clientContext) : IConnectionValidation
{
    private readonly ILogger<ConnectionValidation> _logger = logger;
    private readonly IClientContext _clientContext = clientContext;
    private readonly GlobalEventBusClient _globalEventBusRemote =
        GlobalEventBusClient.Instance!;

    public void ClientInfoValidationConnect()
    {
        var clientInfo = _clientContext.GetClientInfo();

        if (clientInfo == null || !clientInfo.SocketWrapper!.Connected)
        {
            _logger.LogWarning($"Client {clientInfo?.Id} disconnected");
            _globalEventBusRemote.Publish(
                TypeManagerResponseOperations.SocketNotConnected);

            CommunicationStatus.SetConnected(false);
            throw new Exception($"Client {clientInfo?.Id} disconnected");
        }

        if (!clientInfo!.SslStreamWrapper!.IsAuthenticated)
        {
            _logger.LogWarning($"Client {clientInfo.Id} not authenticated");
            _globalEventBusRemote.Publish(
                TypeManagerResponseOperations.SslStreamNotAuthenticated);
        }
    }
}
