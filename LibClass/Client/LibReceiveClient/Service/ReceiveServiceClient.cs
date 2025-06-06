using System.Net.Security;
using System.Net.Sockets;
using System.Text.Json;
using LibEntitiesClient.Entities;
using LibEntitiesClient.Entities.Enum;
using LibHandlerClient.Entities;
using LibHandlerClient.Service;
using LibReceiveClient.Entities;
using LibReceiveClient.Entities.Enum;
using LibReceiveClient.Interface;

namespace LibReceiveClient.Service;

public class ReceiveService : IReceive
{
    private readonly ManagerTypeEventBus _managerTypeEventBus = new();
    private readonly GlobalEventBus _globalEventBus = GlobalEventBus.Instance!;
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
                        await ReceiveAuth(clientInfo, TypeReceive.Default, cts);
                        break;
                    case TypeSocketSsl.Socket:
                        await ReceiveSocket(clientInfo, TypeReceive.Default, cts);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(typeSocketSsl),
                            $"Invalid socket type: {typeSocketSsl}");
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

    public async Task ReceiveDataFileAsync(ClientInfo clientInfo, TypeSocketSsl typeSocketSsl,
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
                        await ReceiveAuth(clientInfo, TypeReceive.File, cts);
                        break;
                    case TypeSocketSsl.Socket:
                        await ReceiveSocket(clientInfo, TypeReceive.File, cts);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(typeSocketSsl),
                            $"Invalid socket type: {typeSocketSsl}");
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

    private async Task ReceiveAuth(ClientInfo clientInfo, TypeReceive typeReceive,
        CancellationToken cts = default)
    {
        if (clientInfo.SslStreamWrapper?.InnerSslStream == null)
            throw new InvalidOperationException("SslStream is not available in ClientInfo.");
        var receive = new ReceiveAuth(clientInfo.SslStreamWrapper.InnerSslStream);
        receive.OnReceivedAct += OnReceivedAtc;
        receive.OnReceivedListAct += OnReceiveList;
        receive.OnClosedAct += OnReceiveAuthClose;

        if (typeReceive == TypeReceive.Default)
        {
            await receive.ReceiveDataAsync(cts);
            return;
        }

        await receive.ReceiveDataFileAsync(cts);
    }

    private async Task ReceiveSocket(ClientInfo clientInfo, TypeReceive typeReceive,
        CancellationToken cts = default)
    {
        if (clientInfo.SocketWrapper?.InnerSocket == null)
            throw new InvalidOperationException("Socket is not available in ClientInfo.");
        var receive = new Receive(clientInfo.SocketWrapper.InnerSocket, cts);
        receive.OnReceivedAct += OnReceivedAtc;
        receive.OnReceivedListAct += OnReceiveList;
        receive.OnClosedAct += OnReceiveSocketClose;

        if (typeReceive == TypeReceive.Default)
        {
            await receive.ReceiveDataAsync(cts);
            return;
        }

        await receive.ReceiveDataAsync(cts);
    }

    private void OnReceivedAtc(JsonElement data)
    {
        Console.WriteLine($"Data received{data}");
        _managerTypeEventBus.PublishEventType(data);
    }

    private void OnReceiveList(List<JsonElement> listData)
    {
        Console.WriteLine($"List received {listData}");
        _managerTypeEventBus.PublishListEventType(listData);
    }

    private void OnReceiveAuthClose(SslStream sslStream)
    {
        _globalEventBus.Publish(sslStream);
    }

    private void OnReceiveSocketClose(Socket socket)
    {
        _globalEventBus.Publish(socket);
    }
}
