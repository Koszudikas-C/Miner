using System.Net.Security;
using System.Text.Json;
using LibCommunicationStatus;
using LibHandler.EventBus;
using LibHandler.ManagerEventBus;
using LibReceive.Entities;
using LibReceive.Interface;
using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Enum;
using LibSsl.Entities;

namespace LibReceive.Service;

public class ReceiveService : IReceive
{
    private readonly ManagerTypeEventBusClient _managerTypeEventBusRemote = new();
    private readonly ManagerTypeEventBusRemote _managerTypeEventBusRemoteRemote = new();
    private readonly GlobalEventBusClient _globalEventBusClient = GlobalEventBusClient.Instance!;
    private readonly GlobalEventBusRemote _globalEventBusRemote = GlobalEventBusRemote.Instance!;
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    public async Task ReceiveDataAsync(ClientInfo clientInfo, TypeRemoteClient typeEnum, int countReceive = 0,
        CancellationToken cts = default)
    {
        await _semaphoreSlim.WaitAsync(cts);
        try
        {
            CommunicationStatus.SetReceiving(true);
            while (!cts.IsCancellationRequested)
            {
                var receive = new ReceiveAuth(clientInfo.SslStream!, cts);

                receive.OnReceivedAct += OnReceivedAtc;
                receive.OnReceivedListAct += OnReceiveList;
                receive.OnClosedAct += OnReceiveClose;

                await receive.ReceiveDataAsync();
                
                if (countReceive-- == -1) break;
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

    private void OnReceivedAtc(JsonElement data)
    {
        Console.WriteLine($"Data received{data}");
        _managerTypeEventBusRemote.PublishEventType(data!);
        _managerTypeEventBusRemoteRemote.PublishEventType(data!);
    }

    private void OnReceiveList(List<JsonElement> listData)
    {
        Console.WriteLine($"List received {listData}");
        _managerTypeEventBusRemote.PublishListEventType(listData);
        _managerTypeEventBusRemoteRemote.PublishListEventType(listData);
    }
    
    private void OnReceiveClose(SslStream sslStream)
    {
        _globalEventBusClient.Publish(sslStream);
        _globalEventBusRemote.Publish(sslStream);
    }
}