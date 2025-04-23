using LibCommunicationStatus;
using LibSocketAndSslStream.Entities.Enum;
using LibSocketAndSslStream.Interface;
using WorkClientBlockChain.Connection.Interface;
using WorkClientBlockChain.Middleware.Interface;

namespace WorkClientBlockChain.Middleware;

public class ConnectionMiddleware(ISocketMiring socketMiring, 
	ILogger<ConnectionMiddleware> logger, IClientContext clientContext) : IConnectionMiddleware
{
    private readonly ISocketMiring _socketMiring = socketMiring;
    private readonly ILogger<ConnectionMiddleware> _logger = logger;
    private readonly IClientContext _clientContext = clientContext;


    public async Task MonitoringConnectionWorkAsync(CancellationToken cts = default)
    {
		try
		{
			var clientInfo = _clientContext.GetClientInfo();
			while (true)
			{
                if (clientInfo is null)
				{
					clientInfo = _clientContext.GetClientInfo();
					await Task.Delay(2000, cts);
					continue;
				}

				if(!CommunicationStatus.IsConnected)
				{
					await _socketMiring.ReconnectAsync((uint)clientInfo.SocketWrapper!.PortRemote,
						0, TypeRemoteClient.Client, TypeAuthMode.RequireAuthentication, cts);
				}

				await Task.Delay(5000, cts);
            }
		}
		catch (Exception ex)
		{

			throw new Exception($"{ex.Message}");
		}
    }
}

