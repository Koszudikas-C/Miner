using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LibHandler.EventBus;
using LibHandler.ManagerEventBus;
using LibReceive.Entities;
using LibReceive.Interface;
using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Enum;

namespace LibReceive.Service;

public class ReceiveServiceClient : IReceive
{
    private readonly ManagerTypeEventBusClient _managerTypeEventBusClient = new();
    private readonly GlobalEventBusClient _globalEventBusClient = GlobalEventBusClient.Instance!;
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    public async Task ReceiveDataAsync(ClientInfo clientInfo, TypeSocketSsl typeSocketSsl,
        int countReceive = 0, CancellationToken cts = default)
    {
        if (clientInfo == null)
            throw new ArgumentNullException(nameof(clientInfo), "ClientInfo cannot be null.");
        await _semaphoreSlim.WaitAsync(cts);
        try
        {
            while (!cts.IsCancellationRequested)
            {
                if (countReceive-- == -1) break;
                switch (typeSocketSsl)
                {
                    case TypeSocketSsl.SslStream:
                        await ReceiveAuth(clientInfo, cts);
                        break;
                    case TypeSocketSsl.Socket:
                        await ReceiveSocket(clientInfo, cts);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(typeSocketSsl), $"Invalid socket type: {typeSocketSsl}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error receiving data: {ex.Message}");
            throw;
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    private async Task ReceiveAuth(ClientInfo clientInfo, CancellationToken cts = default)
    {
        if (clientInfo.SslStreamWrapper?.InnerSslStream == null)
            throw new InvalidOperationException("SslStream is not available in ClientInfo.");
        var receive = new ReceiveAuth(clientInfo.SslStreamWrapper.InnerSslStream);
        receive.OnReceivedAct += OnReceivedAtc;
        receive.OnReceivedListAct += OnReceiveList;
        receive.OnClosedAct += OnReceiveAuthClose;
        await receive.ReceiveDataAsync();
    }

    private async Task ReceiveSocket(ClientInfo clientInfo, CancellationToken cts = default)
    {
        if (clientInfo.SocketWrapper?.InnerSocket == null)
            throw new InvalidOperationException("Socket is not available in ClientInfo.");
        var receive = new Receive(clientInfo.SocketWrapper.InnerSocket, cts);
        receive.OnReceivedAct += OnReceivedAtc;
        receive.OnReceivedListAct += OnReceiveList;
        receive.OnClosedAct += OnReceiveSocketClose;
        await receive.ReceiveDataAsync();
    }

    private void OnReceivedAtc(JsonElement data)
    {
        Console.WriteLine($"Data received{data}");
        _managerTypeEventBusClient.PublishEventType(data!);
    }

    private void OnReceiveList(List<JsonElement> listData)
    {
        Console.WriteLine($"List received {listData}");
        _managerTypeEventBusClient.PublishListEventType(listData);
    }

    private void OnReceiveAuthClose(SslStream sslStream)
    {
        _globalEventBusClient.Publish(sslStream);
    }
    
    private void OnReceiveSocketClose(Socket socket)
    {
        _globalEventBusClient.Publish(socket);
    }
}
