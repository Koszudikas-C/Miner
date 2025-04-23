using System.Net;
using LibRemoteAndClient.Enum;
using ApiRemoteWorkClientBlockChain.Interface;
using LibCommunicationStatus;
using LibCommunicationStatus.Entities;
using LibSocketAndSslStream.Entities;
using LibSocketAndSslStream.Entities.Enum;
using LibSocketAndSslStream.Interface;
using TypeRemoteClient = LibSocketAndSslStream.Entities.Enum.TypeRemoteClient;

namespace ApiRemoteWorkClientBlockChain.Service;

public class ManagerConnectionService(ILogger<ManagerConnectionService> logger, ISocketMiring socketMiring,
    IAuthSsl authSsl, IManagerClient managerClient) : IManagerConnection
{

    public async Task<ApiResponse<object>> InitializeAsync(ConnectionConfig connectionConfig, TypeAuthMode typeAuthMode,
        CancellationToken cts = default)
    {
        if(CommunicationStatus.IsConnected)
            return InstanceApiResponse<object>(HttpStatusCode.Conflict, false,
                $"Remote server already {connectionConfig.Port} in use", null!);
        try
        {
            await socketMiring!.InitializeAsync(connectionConfig.Port, connectionConfig.MaxConnections,
                TypeRemoteClient.Remote, typeAuthMode, cts).ConfigureAwait(false);

            for (var i = 0; i < 5; i++)
            {
                if (CommunicationStatus.IsConnecting) break;
                
                await Task.Delay(1000, cts).ConfigureAwait(false);

                if (i == 4)
                    return InstanceApiResponse<object>(HttpStatusCode.GatewayTimeout, false, 
                        "Service initialization failed due to a timeout while attempting to establish" +
                        " communication with the remote server. Please check the connection settings and try again.", null!);
            }
            
            return InstanceApiResponse<object>(HttpStatusCode.OK, true, 
                $"Successful service initialization type {typeAuthMode.ToString()}", null!);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            logger.LogError($"Error starting SocketMiringService in mode {typeAuthMode}," +
                             $" Port:{connectionConfig.Port}," +
                             $" MaxConnections:{connectionConfig.MaxConnections}: {e.Message}");
            CommunicationStatus.SetConnected(false);
            return InstanceApiResponse<object>(HttpStatusCode.InternalServerError, false,
                "Service initialization failed. Verify the settings and try again.", null!);
        }
    }
    
    private static ApiResponse<T> InstanceApiResponse<T>(HttpStatusCode statusCode, bool sucess, 
        string message, IEnumerable<T> data, List<string>? errors = null) 
        => new ApiResponse<T>(statusCode, sucess, message, data, errors);
}