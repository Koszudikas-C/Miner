using System.Net.Security;
using System.Net.Sockets;
using LibRemoteAndClient.Enum;
using LibSsl.Enum;

namespace LibSsl.Interface;

public interface IAuthSsl
{
    Task? AuthenticateAsync(Socket socket, TypeRemoteClient typeRemoteClient, 
        Guid clientId, CancellationToken cancellationToken = default);
}