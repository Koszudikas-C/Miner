using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using LibSocket.Entities.Enum;
using LibSocks5.Entities;

namespace LibSocket.Connection;

public abstract class ListenerBase(
    AddressFamily addressFamily,
    SocketType socketType,
    ProtocolType protocolType)
{
    public Socket Socket { get; protected set; } = new(
        addressFamily, socketType, protocolType);

    public int Port { get; protected set; }

    public bool Listening { get; protected set; }

    public abstract Task StartAsync(TypeAuthMode typeAuthMode, uint port, 
        int maxConnections = 0, CancellationToken cts = default);

    public abstract Task ReconnectAsync(TypeAuthMode typeAuthMode,
        CancellationToken cts = default);

    protected Socks5Options? Socks5Options { get; private set; } = GetSocks5Options();

    public abstract void Stop();

    protected static async Task<string> GetIpAddress()
    {
        var hostVariable= Environment.GetEnvironmentVariable("SERVICE_NAME_REMOTE_BLOCK_SSL");
        
        if (string.IsNullOrWhiteSpace(hostVariable)) 
            throw new ArgumentNullException($"Check connection host");
        
        var host = await Dns.GetHostAddressesAsync(hostVariable);
        return host[0].ToString()!;

    }

    private static Socks5Options GetSocks5Options()
    {
        string GetEnv(string name)
            => Environment.GetEnvironmentVariable(name)
               ?? throw new InvalidOperationException($"Environment variable name is not defined");

        int GetEnvInt(string name)
            => int.TryParse(GetEnv(name), out var value)
                ? value
                : throw new InvalidOperationException(
                    $"Environment variable '{name}' should be a valid integer.");

        return new Socks5Options(
            GetEnv("PROXY_HOST"),
            GetEnvInt("PROXY_PORT"),
            GetEnv("SERVICE_NAME_WORK_SERVICE"),
            GetEnvInt("REMOTE_PORT"),
            GetEnv("REMOTE_USERNAME_TOR"),
            GetEnv("REMOTE_PASSWORD_TOR")
        );
    }

    protected void OnConnectedAct(Socket socket) => ConnectedAct?.Invoke(socket);

    public event Action<Socket>? ConnectedAct;
}