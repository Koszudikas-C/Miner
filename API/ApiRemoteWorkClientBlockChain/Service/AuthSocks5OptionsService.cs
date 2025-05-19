using ApiRemoteWorkClientBlockChain.Entities.Interface;
using ApiRemoteWorkClientBlockChain.Utils;
using LibClassProcessOperations.Interface;
using LibDto.Dto;
using LibHandler.EventBus;
using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Enum;
using LibSend.Interface;

namespace ApiRemoteWorkClientBlockChain.Service;

internal class AuthSocks5OptionsService(
    ISend<ParamsSocks5Dto> sendSocks5,
    ILogger<AuthSocks5OptionsService> logger,
    IClientConnected clientConnected) : IProcessOptions
{
    private readonly GlobalEventBusRemote _globalEventBusRemote = GlobalEventBusRemote.Instance!;

    /// <summary>
    /// Process method is always default in a matter of parameters. Summarizing automated.
    /// </summary>
    public async Task ProcessAsync(CancellationToken cts = default)
    {
        await SendParamsSocks5Async(PreparationSocks5Dto(), LastClientInfoConnected(), cts);
        logger.LogInformation("Automated SOCKS5 parameters successfully sent!.");
    }

    public async Task EditSendParamsSocks5(ParamsSocks5Dto paramsSocks5Dto, ClientInfo clientInfo,
        CancellationToken cts = default)
    {
        await SendParamsSocks5Async(paramsSocks5Dto, clientInfo, cts);
        logger.LogInformation("SOCKS5 Parameter Manual Shipping Made Successfully!.");
    }

    private static ParamsSocks5Dto PreparationSocks5Dto() => AuthSocks5Util.GetParamsSocks5DtoDefault();
    private ClientInfo LastClientInfoConnected() => clientConnected.GetClientInfos().LastOrDefault()!;

    private async Task SendParamsSocks5Async(ParamsSocks5Dto paramsSocks5Dto,
        ClientInfo clientInfo = null!, CancellationToken cts = default)
    {
        try
        {
            await sendSocks5.SendAsync(paramsSocks5Dto, clientInfo, TypeSocketSsl.SslStream, cts);
        }
        catch (Exception e)
        {
            throw new Exception(
                $"It was not possible to send the parameters to configure SCOKS5. Error:{e.Message}");
        }
    }
}