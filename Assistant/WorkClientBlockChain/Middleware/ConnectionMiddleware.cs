using LibCommunicationStatus;
using LibSocketAndSslStream.Entities.Enum;
using LibSocketAndSslStream.Interface;
using WorkClientBlockChain.Connection.Interface;
using WorkClientBlockChain.Middleware.Interface;

namespace WorkClientBlockChain.Middleware;

public class ConnectionMiddleware(ISocketMiring socketMiring, 
	ILogger<ConnectionMiddleware> logger, IClientConnected clientConnected, IAuthSsl authSsl) : IConnectionMiddleware
{
    private readonly ISocketMiring _socketMiring = socketMiring;
    private readonly ILogger<ConnectionMiddleware> _logger = logger;
    private readonly IClientConnected _clientConnected = clientConnected;
    private readonly IAuthSsl _authSsl = authSsl; 


    public async Task MonitoringConnectionWorkAsync(CancellationToken cts = default)
    {
		try
		{
			var clientInfo = _clientConnected.GetClientInfo();
			while (true)
			{
                if (clientInfo is null)
				{
					clientInfo = _clientConnected.GetClientInfo();
					await Task.Delay(2000, cts);
					continue;
				}

				if(!CommunicationStatus.IsConnected)
				{
					Console.WriteLine("Reconnecting...");
					
					clientInfo.SslStreamWrapper?.InnerSslStream!.Close();
					
					await _socketMiring.ReconnectAsync((uint)clientInfo.SocketWrapper!.PortRemote,
						0, TypeAuthMode.RequireAuthentication, cts);
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

