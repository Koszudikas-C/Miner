using LibSocketAndSslStream.Entities.Enum;

namespace LibSocketAndSslStream.Interface;

public interface IAuthSsl
{
    Task AuthenticateAsync(ISocketWrapper socket, TypeRemoteClient typeRemoteClient, 
        Guid clientId, CancellationToken cts = default);
}