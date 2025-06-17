using System.Net;
using System.Net.Sockets;
using LibCommunicationStateClient.Entities.Enum;
using LibHandlerClient.Entities;
using LibSocketAndSslStreamClient.Entities;
using LibSocketAndSslStreamClient.Entities.Enum;
using LibSocketAndSslStreamClient.Interface;
using LibSocks5Client.Interface;
using Microsoft.Extensions.Logging;

namespace LibSocketClient.Service;

public class ListenerService(
    IConfigVariable configVariable,
    ISocks5Options socks5Options,
    ISocks5 socks5,
    ILogger<ListenerService> logger, IListenerWrapper listener)
    : IListener
{
    private readonly GlobalEventBus _globalEventBus = GlobalEventBus.Instance;
    private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

    public event Func<Socket, CancellationToken, Task>? ConnectedAct;

    public async Task StartAsync(TypeAuthMode typeAuthMode, uint port,
        CancellationToken cts = default)
    {
        listener.Listener.Port = (int)port;
        await ConnectWithRetryAsync(typeAuthMode, cts);
    }

    public async Task ReconnectAsync(Socket socket, TypeAuthMode typeAuthMode,
        CancellationToken cts = default)
    {
        await Task.Delay(TimeSpan.FromSeconds(10), cts);
        if (!socket.Connected)
        {
            logger.LogInformation("Client tried to make the" +
                                  " reconnection without being connected");
          throw new SocketException();
        }

        await CheckMaxReconnection(cts);
        await listener.Listener.SocketClient.DisconnectAsync(true, cts);
        _semaphoreSlim.Release();
        await ConnectWithRetryAsync(typeAuthMode, cts);
    }

    private async Task CheckMaxReconnection(CancellationToken cts = default)
    {
        if (listener.Listener.CountReconnection++ != 3)
        {
            logger.LogInformation("Amount of attempts " +
                                  "used in reconnecting " +
                                  "with Remote. Attempt: {count}"
                , listener.Listener.CountReconnection);
            return;
        }

        logger.LogInformation("The maximum of reconnecting attempt by " +
                              "reaching waiting for the release.");

        await Task.Delay(TimeSpan.FromMinutes(5), cts);

        listener.Listener.CountReconnection = 0;
    }

    private async Task ConnectWithRetryAsync(TypeAuthMode typeAuthMode, CancellationToken cts)
    {
        CheckNullSocketClient();
        await (typeAuthMode == TypeAuthMode.RequireAuthentication
            ? ConnectDefaultRemoteAsync(cts)
            : ConnectSocks5RemoteAsync(cts));
    }

    private async Task ConnectDefaultRemoteAsync(CancellationToken cts = default)
    {
        var resultConfigVariable = configVariable.GetConfigVariable();
        var data = (ConfigVariable)resultConfigVariable.GetData();
        var ip = await Dns.GetHostEntryAsync(data.RemoteSslBlock!, cts);
        do
        {
            try
            {
                await _semaphoreSlim.WaitAsync(cts);
                
                logger.LogInformation($"trying to connect to the ssl server " +
                                      $"{ip.AddressList[0].ToString()}: {data.RemoteSslBlockPort}");

                await listener.Listener.SocketClient.ConnectAsync(ip.AddressList[0].ToString(),
                    data.RemoteSslBlockPort,
                    cts);
                
                
                logger.LogInformation("Connected to the server");
                listener.Listener.Listening = true;

                await OnConnectedActAsync(cts);
                Console.WriteLine($"Thread de saida" +
                                  $" do ConnectDefaultRemoteAsync. {Environment.CurrentManagedThreadId}");
                break;
            }
            catch (SocketException)
            {
                Console.WriteLine("Connection error trying again in 5 seconds");
                await Task.Delay(5000, cts);
            }
            catch (OperationCanceledException e)
            {
                logger.LogWarning("Authentication operation exceeded with Remote. Error: {Message}", e);

                await ReconnectAsync(listener.Listener.SocketClient,
                    TypeAuthMode.RequireAuthentication, cts).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                logger.LogCritical("There was an unprecedented error in " +
                                   "the initiation of the entire application.");
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        } while (!cts.IsCancellationRequested);
    }

    private async Task ConnectSocks5RemoteAsync(CancellationToken cts = default)
    {
        do
        {
            try
            {
                var resultConfigSocks5 = socks5Options.GetSocks5Options();

                logger.LogInformation($"trying to connect to the onion server {resultConfigSocks5.DestinationHost}");

                await socks5.ConnectAsync(() => listener.Listener.SocketClient, resultConfigSocks5, cts);

                await OnConnectedActAsync(cts).ConfigureAwait(false);
                break;
            }
            catch (SocketException)
            {
                logger.LogWarning("Connection error trying again in 5 seconds");
                await Task.Delay(5000, cts);
            }
            catch (Exception e)
            {
                logger.LogCritical("There was an unprecedented error in " +
                                   "the initiation of the entire application.");
                Disposable();
                _globalEventBus.Publish(ApplicationState.Restart, cts);
                throw new Exception(e.Message);
            }
        } while (!cts.IsCancellationRequested);
    }

    public void Disposable()
    {
        if (!listener.Listener.Listening) return;

        listener.Listener.Listening = false;
        listener.Listener.SocketClient.Dispose();
    }

    private async Task OnConnectedActAsync(CancellationToken cts = default)
    {
        CheckNullSocketClient();
        SetConfigSocket();
        listener.Listener.Listening = true;

        if (ConnectedAct is not null)
            await ConnectedAct.Invoke(listener.Listener.SocketClient, cts);
    }

    private void CheckNullSocketClient()
    {
        if (listener.Listener.SocketClient is null)
        {
            throw new InvalidOperationException("Check the customer's" +
                                                " socket is passing with a null value");
        }
    }

    private void SetConfigSocket()
    {
        CheckNullSocketClient();
        listener.Listener.SocketClient.ReceiveTimeout = 10000;
        listener.Listener.SocketClient.SendTimeout = 10000;
    }
}