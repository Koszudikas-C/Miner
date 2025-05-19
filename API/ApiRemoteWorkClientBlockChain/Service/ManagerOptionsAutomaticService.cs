using System.ComponentModel;
using System.Net.Sockets;
using ApiRemoteWorkClientBlockChain.Entities;
using ApiRemoteWorkClientBlockChain.Utils;
using LibClassManagerOptions.Entities;
using LibClassManagerOptions.Entities.Enum;
using LibClassManagerOptions.Interface;
using LibDto.Dto;
using LibHandler.EventBus;
using LibSend.Interface;
using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Enum;

namespace ApiRemoteWorkClientBlockChain.Service;

public class ManagerOptionsAutomaticService<T> : IManagerOptions<T>
{
    private readonly ISend<TypeManagerOptions> _send;
    private readonly ILogger<ManagerOptionsAutomaticService<T>> _logger;
    private readonly ISend<ParamsManagerOptionsDto<T>> _sendParamsManagerOptions;
    private readonly GlobalEventBusRemote _globalEventBusRemote = GlobalEventBusRemote.Instance!;
    private readonly ClientConnected _clientConnected = ClientConnected.Instance;
    
    public ManagerOptionsAutomaticService(ISend<TypeManagerOptions> send,
        ILogger<ManagerOptionsAutomaticService<T>> logger,
        ISend<ParamsManagerOptionsDto<T>> sendParamsManagerOptions)
    {
        _send = send;
        _logger = logger;
        _sendParamsManagerOptions = sendParamsManagerOptions;
        _globalEventBusRemote.Subscribe<ParamsManagerOptionsResponseDto>(
            (handler) => _= ResponseOptionsAsync(handler));
    }
    
    public async Task InitializeOptionsAsync(ParamsManagerOptions<T> paramsManagerOptions, 
        CancellationToken cts = default)
    {
        var clientInfo = GetClientInfoManager();
        var typeSocketSsl = GetSocketSsl(clientInfo);
        
        switch (paramsManagerOptions.TypeManagerOptions)
        {
            case TypeManagerOptions.AuthSocks5:
                var result = PreparationAuthSocks5Options(paramsManagerOptions);
                await _sendParamsManagerOptions.SendAsync(result, clientInfo, typeSocketSsl, cts);
                _logger.LogInformation("Successfully sent the command for Socks5 attentive!");
                break;
            case TypeManagerOptions.CheckAppClientBlockChain:
                await _send.SendAsync(TypeManagerOptions.CheckAppClientBlockChain,
                    clientInfo, typeSocketSsl, cts);
                break;
            case TypeManagerOptions.DownloadAppClientBlockChain:
                await _send.SendAsync(TypeManagerOptions.DownloadAppClientBlockChain,
                    clientInfo, typeSocketSsl, cts);
                break;
            case TypeManagerOptions.Logs:
                await _send.SendAsync(TypeManagerOptions.Logs, clientInfo, typeSocketSsl, cts);
                break;
            case TypeManagerOptions.StatusConnection:
                await _send.SendAsync(TypeManagerOptions.StatusConnection, clientInfo,
                    typeSocketSsl, cts);
                break;
            case TypeManagerOptions.StatusTransaction:
                await _send.SendAsync(TypeManagerOptions.StatusTransaction, clientInfo,
                    typeSocketSsl, cts);
                break;
            case TypeManagerOptions.CancelOperations:
                await _send.SendAsync(TypeManagerOptions.CancelOperations, clientInfo,
                    typeSocketSsl, cts);
                break;
            case TypeManagerOptions.Error:
            default:
                await _send.SendAsync(TypeManagerOptions.Error, clientInfo, typeSocketSsl, cts);
                break;
        }
    }

    private static ParamsManagerOptionsDto<T> PreparationAuthSocks5Options(ParamsManagerOptions<T> paramsManagerOptions)
    {
        return ManagerOptionsUtil.GetParamsManagerOptionsDto(paramsManagerOptions);
    }

    public async Task ResponseOptionsAsync(ParamsManagerOptionsResponseDto paramsManagerOptionsResponseDto,
        CancellationToken cts = default)
    {
        var paramsManagerOptionsResponse = ManagerOptionsUtil.GetParamsManagerOptionsResponse(paramsManagerOptionsResponseDto);
        
        switch (paramsManagerOptionsResponse.TypeManagerOptionsResponse)
        {
            case TypeManagerOptionsResponse.Success:
                _logger.LogInformation($"Task accomplished successfully. object type{paramsManagerOptionsResponse.TypeName}");
                break;
            case TypeManagerOptionsResponse.NotFound:
                _logger.LogInformation($"Task was not found. Error: {paramsManagerOptionsResponse.TypeName}");
                break;
            case TypeManagerOptionsResponse.InvalidRequest:
                break;
            case TypeManagerOptionsResponse.Error:
                break;
            case TypeManagerOptionsResponse.Unauthorized:
                break;
            case TypeManagerOptionsResponse.Timeout:
                break;
            case TypeManagerOptionsResponse.AlreadyExists:
                break;
            case TypeManagerOptionsResponse.ValidationFailed:
                break;
            case TypeManagerOptionsResponse.PartialSuccess:
                break;
            case TypeManagerOptionsResponse.Pending:
                break;
            case TypeManagerOptionsResponse.SslStreamNotAuthenticated:
                break;
            case TypeManagerOptionsResponse.SocketNotConnected:
                break;
            case TypeManagerOptionsResponse.PortNotOpen:
                break;
            case TypeManagerOptionsResponse.TypeNotDefined:
                break;
            case TypeManagerOptionsResponse.ProcessNotKill:
                break;
            default:
                throw new InvalidEnumArgumentException(nameof(paramsManagerOptionsResponse));
        }

        await Task.CompletedTask;
    }
    
    private ClientInfo GetClientInfoManager()
    {
        var clientInfo = _clientConnected.GetClientInfoLastRequirement();

        if (clientInfo != null! && clientInfo.SocketWrapper!.Connected) return clientInfo;
        
        clientInfo!.SocketWrapper!.InnerSocket.Close();
        throw new SocketException();
    }

    private static TypeSocketSsl GetSocketSsl(ClientInfo? clientInfo)
    {
        if(clientInfo is not null && clientInfo.SslStreamWrapper!.IsAuthenticated)
            return TypeSocketSsl.SslStream;
        
        return TypeSocketSsl.Socket;
    }
}