using LibDto.Dto;
using LibCommunicationStatus.Entities;

namespace ApiRemoteWorkClientBlockChain.Interface
{
    public interface IPosAuth
    {
        Task<ApiResponse<object>> SendDataAsync(ConfigCryptographDto configCryptographDto, Guid clientId, CancellationToken cts = default);
    }
}
