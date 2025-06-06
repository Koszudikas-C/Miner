using ApiRemoteWorkClientBlockChain.Entities;
using ApiRemoteWorkClientBlockChain.Entities.Interface;
using ApiRemoteWorkClientBlockChain.Factory.Abstract;
using ApiRemoteWorkClientBlockChain.Interface;
using ApiRemoteWorkClientBlockChain.Service;
using LibDtoRemote.Dto;
using LibSendRemote.Interface;
using LibSendRemote.Service;

namespace ApiRemoteWorkClientBlockChain.Factory;

public class AuthSocks5OperationFactory : ProcessFactory
{
    private readonly ISend<ParamsSocks5Dto> _sendSocks5 = new SendService<ParamsSocks5Dto>();
    private static readonly ILoggerFactory LogerFactory = new LoggerFactory();
    private readonly ILogger<AuthSocks5OptionsService> _logger = new Logger<AuthSocks5OptionsService>(LogerFactory);
    private readonly IClientConnected _clientConnected = ClientConnected.Instance;
    
    public override IProcessOptions CreateProcess() => new AuthSocks5OptionsService(_sendSocks5, _logger, _clientConnected);
}
