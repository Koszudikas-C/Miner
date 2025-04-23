using System.Net;
using System.Net.Sockets;
using LibRemoteAndClient.Entities.Client;
using LibSearchFile;
using LibSearchFile.Enum;
using LibSocketAndSslStream.Entities.Enum;
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

    public abstract Task ReconnectAsync(TypeAuthMode typeAuthMode, uint port,
        int maxConnection,
        CancellationToken cts = default);

    protected Socks5Options? Socks5Options { get; private set; } = GetSocks5Options();

    public abstract void Stop();

    protected static ConfigVariable GetIpAddress()
    {
        var configVariable = SearchManager.SearchFile(TypeFile.ConfigVariable) as ConfigVariable
                           ?? new ConfigVariable { RemoteSslHost = "189.78.182.54", RemoteSslPort = 5051 };

        if (string.IsNullOrWhiteSpace(configVariable.RemoteSslHost))
            throw new ArgumentNullException($"Check connection host");

        return configVariable;
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