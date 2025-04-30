using System.Net.Security;
using System.Net.Sockets;
using System.Text.Json;
using LibCommunicationStatus;
using LibHandler.EventBus;
using LibHandler.ManagerEventBus;
using LibReceive.Entities;
using LibReceive.Interface;
using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Enum;

namespace LibReceive.Service;

public class ReceiveService : IReceive
{
    private readonly ManagerTypeEventBusClient _managerTypeEventBusClient = new();
    private readonly ManagerTypeEventBusRemote _managerTypeEventBusRemote = new();
    private readonly GlobalEventBusClient _globalEventBusClient = GlobalEventBusClient.Instance!;
    private readonly GlobalEventBusRemote _globalEventBusRemote = GlobalEventBusRemote.Instance!;
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    public async Task ReceiveDataAsync(ClientInfo clientInfo, TypeSocketSsl typeSocketSsl,
        int countReceive = 0,
        CancellationToken cts = default)
    {
        await _semaphoreSlim.WaitAsync(cts);
        try
        {
            CommunicationStatus.SetReceiving(true);
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
                        throw new ArgumentOutOfRangeException(nameof(typeSocketSsl), typeSocketSsl, null);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error receiving data {ex.Message}");
            throw;
        }
        finally
        {
            CommunicationStatus.SetReceiving(false);
            _semaphoreSlim.Release();
        }
    }

    private async Task ReceiveAuth(ClientInfo clientInfo, CancellationToken cts = default)
    {
        var receive = new ReceiveAuth(clientInfo.SslStreamWrapper!.InnerSslStream!);
        receive.OnReceivedAct += OnReceivedAtc;
        receive.OnReceivedListAct += OnReceiveList;
        receive.OnClosedAct += OnReceiveAuthClose;
        await receive.ReceiveDataAsync();
    }

    private async Task ReceiveSocket(ClientInfo clientInfo, CancellationToken cts = default)
    {
        var receive = new Receive(clientInfo.SocketWrapper!.InnerSocket!, cts);
        receive.OnReceivedAct += OnReceivedAtc;
        receive.OnReceivedListAct += OnReceiveList;
        receive.OnClosedAct += OnReceiveClose;
        await receive.ReceiveDataAsync(cts);
    }

    private void OnReceivedAtc(JsonElement data)
    {
        Console.WriteLine($"Data received{data}");
        _managerTypeEventBusClient.PublishEventType(data!);
        _managerTypeEventBusRemote.PublishEventType(data!);
    }

    private void OnReceiveList(List<JsonElement> listData)
    {
        Console.WriteLine($"List received {listData}");
        _managerTypeEventBusClient.PublishListEventType(listData);
        _managerTypeEventBusRemote.PublishListEventType(listData);
    }
    
    private void OnReceiveAuthClose(SslStream sslStream)
    {
        _globalEventBusClient.Publish(sslStream);
        _globalEventBusRemote.Publish(sslStream);
    }
    
    private void OnReceiveClose(Socket socket)
    {
        _globalEventBusClient.Publish(socket);
        _globalEventBusRemote.Publish(socket);
    }
}