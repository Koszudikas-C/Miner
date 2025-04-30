using LibCommunicationStatus;
using LibSocketAndSslStream.Entities.Enum;
using LibSocketAndSslStream.Interface;
using WorkClientBlockChain.Connection.Interface;
using WorkClientBlockChain.Middleware.Interface;

namespace WorkClientBlockChain.Middleware;

public class ConnectionMiddleware(ISocketMiring socketMiring, 
	ILogger<ConnectionMiddleware> logger, IClientContext clientContext, IAuthSsl authSsl) : IConnectionMiddleware
{
    private readonly ISocketMiring _socketMiring = socketMiring;
    private readonly ILogger<ConnectionMiddleware> _logger = logger;
    private readonly IClientContext _clientContext = clientContext;
    private readonly IAuthSsl _authSsl = authSsl; 


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
					Console.WriteLine("Reconectando...");
					
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

