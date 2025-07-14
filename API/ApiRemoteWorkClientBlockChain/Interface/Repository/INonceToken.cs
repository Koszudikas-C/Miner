using LibCommunicationStateRemote.Entities;
using LibRemoteAndClient.Entities.Remote.Client;

namespace ApiRemoteWorkClientBlockChain.Interface.Repository;

public interface INonceToken : IRepositoryBase<int, GuidTokenAuth>
{
    ApiResponse<GuidTokenAuth> GetByNonce(Guid nonce);
}