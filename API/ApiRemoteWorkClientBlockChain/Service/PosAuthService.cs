using System.Net;
using ApiRemoteWorkClientBlockChain.Entities.Interface;
using ApiRemoteWorkClientBlockChain.Interface;
using LibCommunicationStatus;
using LibCommunicationStatus.Entities;
using LibCryptography.Entities;
using LibDto.Dto;
using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Enum;
using LibSend.Interface;
using Microsoft.AspNetCore.Authentication;

namespace ApiRemoteWorkClientBlockChain.Service;

public class PosAuthService(
    ILogger<PosAuthService> logger,
    IClientConnected clientConnected,
    ISend<ConfigCryptographDto> sendConfigCryptographDto)
    : IPosAuth
{
    public async Task<ApiResponse<object>> SendDataAsync(ConfigCryptographDto configCryptographDto, Guid clientId, CancellationToken cts = default)
    {
        try
        {
            var clientInfo = clientConnected.GetClientInfo(clientId);
            
            if (!clientInfo.SslStreamWrapper!.IsAuthenticated)
                return InstanceApiResponse<object>(HttpStatusCode.Unauthorized, false, "Unauthorized client: " + clientInfo.Id, null!);

            await sendConfigCryptographDto.SendAsync(configCryptographDto, clientInfo, TypeSocketSsl.SslStream, cts);
            return InstanceApiResponse<object>(HttpStatusCode.OK, true, "Successful send", null!);
        }
        catch (Exception e)
        {
            logger.LogError($"Error sending data: {e.Message}");
            return InstanceApiResponse<object>(HttpStatusCode.InternalServerError, false, "Error sending data", null!);
        }
    }

    private static ApiResponse<T> InstanceApiResponse<T>(HttpStatusCode statusCode, bool success, string message, IEnumerable<T> data, List<string>? errors = null) 
        => new ApiResponse<T>(statusCode, success, message, data, errors);
}
