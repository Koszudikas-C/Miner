using LibDtoRemote.Dto;

namespace ApiRemoteWorkClientBlockChain.Interface;

public interface IPosAuth
{
  Task SendDataAsync(
    ConfigCryptographDto configCryptographDto, Guid clientId, CancellationToken cts = default);
}
