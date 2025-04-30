using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibSocks5.Entities.Enum;
using LibSocks5.Entities;
using LibSocks5.Interface;

namespace LibSocks5.Service;

public class Socks5Service : ISocks5
{
    public async Task<Socket> ConnectAsync(Func<Socket> socketFactory, Socks5Options options,
        CancellationToken cts = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        var socket = socketFactory();
        await socket.ConnectAsync(options.ProxyHost, options.ProxyPort, cts);
        await SelectAuth(socket, options, cts); 
        await Connect(socket, options, cts);

        return socket;
    }

    private static async Task SelectAuth(Socket socket, Socks5Options options, CancellationToken cts = default)
    {
        var buffer = new byte[4]
        {
            5,
            2,
            Socks5Constants.AuthMethodNoAuthenticationRequired, Socks5Constants.AuthMethodUsernamePassword
        };
        await socket.SendAsync(buffer, SocketFlags.None, cts);
        
        var response = new byte[2];
        var read = await socket.ReceiveAsync(response, SocketFlags.None);
        if (read != 2)
            throw new Exception($"Failed to select an authentication method, the server sent {read} bytes.");

        switch (response[1])
        {
            case Socks5Constants.AuthMethodReplyNoAcceptableMethods:
                socket.Close();
                throw new Exception(
                    "The proxy destination does not accept the supported proxy client authentication methods.");
            case Socks5Constants.AuthMethodUsernamePassword when options.Auth == AuthType.None:
                socket.Close();
                throw new Exception("The proxy destination requires a username and password for authentication.");
        }

        if (response[1] == Socks5Constants.AuthMethodNoAuthenticationRequired)
            return;

        await PerformAuth(socket, options, cts);
    }

    private static async Task PerformAuth(Socket socket, Socks5Options options, CancellationToken cts = default)
    {
        var buffer = ConstructAuthBuffer(options.Credentials!.UserName, options.Credentials.Password);
        await socket.SendAsync(buffer, SocketFlags.None, cts);

        var response = new byte[2];
        var read = await socket.ReceiveAsync(response, SocketFlags.None);
        if (read != 2)
            throw new Exception($"Failed to perform authentication, the server sent {read} bytes.");

        if (response[1] != 0)
        {
            socket.Close();
            throw new Exception("Proxy authentication failed.");
        }
    }

    private static async Task Connect(Socket socket, Socks5Options options, CancellationToken cts = default)
    {
        
        var addressType = GetDestAddressType(options.DestinationHost);
        var destAddr = GetDestAddressBytes(addressType, options.DestinationHost);
        var destPort = GetDestPortBytes(options.DestinationPort);

        var buffer = new byte[7 + options.DestinationHost.Length];
        buffer[0] = 5;
        buffer[1] = Socks5Constants.CmdConnect;
        buffer[2] = Socks5Constants.Reserved;
        buffer[3] = addressType;
        destAddr!.CopyTo(buffer, 4);
        destPort.CopyTo(buffer, 4 + destAddr.Length);

        await socket.SendAsync(buffer, SocketFlags.None, cts);
        
        var response = new byte[255];
        await socket.ReceiveAsync(response, SocketFlags.None);

        if (response[1] != Socks5Constants.CmdReplySucceeded)
            HandleProxyCommandError(response, options.DestinationHost, options.DestinationPort);
    }

    private static void HandleProxyCommandError(byte[] response, string destinationHost, int destinationPort)
    {
        var replyCode = response[1];
        var proxyErrorText = replyCode switch
        {
            Socks5Constants.CmdReplyGeneralSocksServerFailure => "a general socks destination failure occurred",
            Socks5Constants.CmdReplyConnectionNotAllowedByRuleset =>
                "the connection is not allowed by proxy destination rule set",
            Socks5Constants.CmdReplyNetworkUnreachable => "the network was unreachable",
            Socks5Constants.CmdReplyHostUnreachable => "the host was unreachable",
            Socks5Constants.CmdReplyConnectionRefused => "the connection was refused by the remote network",
            Socks5Constants.CmdReplyTtlExpired => "the time to live (TTL) has expired",
            Socks5Constants.CmdReplyCommandNotSupported =>
                "the command issued by the proxy client is not supported by the proxy destination",
            Socks5Constants.CmdReplyAddressTypeNotSupported => "the address type specified is not supported",
            _ => string.Format(CultureInfo.InvariantCulture,
                "an unknown SOCKS reply with the code value '{0}' was received",
                replyCode.ToString(CultureInfo.InvariantCulture)),
        };
        var exceptionMsg = string.Format(CultureInfo.InvariantCulture,
            "proxy error: {0} for destination host {1} port number {2}.",
            proxyErrorText, destinationHost, destinationPort);

        throw new Exception(exceptionMsg);
    }

    private static byte[] ConstructAuthBuffer(string username, string password)
    {
        var credentials = new byte[3 + username.Length + password.Length];

        credentials[0] = 0x01;
        credentials[1] = (byte)username.Length;
        Array.Copy(Encoding.ASCII.GetBytes(username), 0, credentials,
            2, username.Length);
        credentials[username.Length + 2] = (byte)password.Length;
        Array.Copy(Encoding.ASCII.GetBytes(password), 0, credentials,
            2, password.Length);

        return credentials;
    }

    private static byte GetDestAddressType(string host)
    {
        if (!IPAddress.TryParse(host, out var ipAddr))
            return Socks5Constants.AddrtypeDomainName;

        switch (ipAddr.AddressFamily)
        {
            case AddressFamily.InterNetwork:
                return Socks5Constants.AddrtypeIpv4;
            case AddressFamily.InterNetworkV6:
                return Socks5Constants.AddrtypeIpv6;
            default:
                throw new Exception(
                    $"The host addess {host} of type '{Enum.GetName(typeof(AddressFamily), ipAddr.AddressFamily)}' is not a supported address type.\n" +
                    "The supported types are InterNetwork and InterNetworkV6.");
        }
    }

    private static byte[]? GetDestAddressBytes(byte addressType, string host)
    {
        switch (addressType)
        {
            case Socks5Constants.AddrtypeIpv4:
            case Socks5Constants.AddrtypeIpv6:
                return IPAddress.Parse(host).GetAddressBytes();
            case Socks5Constants.AddrtypeDomainName:
                var bytes = new byte[host.Length + 1];
                bytes[0] = Convert.ToByte(host.Length);
                Encoding.ASCII.GetBytes(host).CopyTo(bytes, 1);
                return bytes;
            default:
                return null;
        }
    }

    private static byte[] GetDestPortBytes(int value)
    {
        return
        [
            Convert.ToByte(value / 256),
            Convert.ToByte(value % 256)
        ];
    }
}