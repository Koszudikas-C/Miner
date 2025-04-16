using System.Net.Security;
using System.Net.Sockets;
using LibSsl.Auth;

namespace UpdateClientService.Connection;

public sealed class AuthSsl
{
    private static AuthSsl? _instance;
    private static readonly object _lock = new();

    private readonly AuthClient _authClient;

    private AuthSsl(Socket socket)
    {
        _authClient = new AuthClient(socket);
    }

    public static AuthSsl Instance(Socket socket)
    {
        if (_instance != null) return _instance;

        lock (_lock)
        {
            _instance ??= new AuthSsl(socket);
        }

        return _instance;
    }
    
    public AuthClient GetAuthClient() => _authClient; 
}
