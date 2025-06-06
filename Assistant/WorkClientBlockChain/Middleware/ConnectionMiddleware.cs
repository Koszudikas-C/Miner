using LibCommunicationStateClient.Entities;
using LibSocketAndSslStreamClient.Entities.Enum;
using LibSocketAndSslStreamClient.Interface;
using WorkClientBlockChain.Connection.Interface;
using WorkClientBlockChain.Middleware.Interface;

namespace WorkClientBlockChain.Middleware;

public class ConnectionMiddleware(
  ISocketMiring socketMiring,
  ILogger<ConnectionMiddleware> logger,
  IClientConnected clientConnected) : IConnectionMiddleware
{
  private readonly ISocketMiring _socketMiring = socketMiring;
  private readonly ILogger<ConnectionMiddleware> _logger = logger;
  private readonly IClientConnected _clientConnected = clientConnected;


  public async Task MonitoringConnectionWorkAsync(CancellationToken cts = default)
  {
    _ = Task.Run(async () => { await ReconnectAuthAsync(cts); }, cts);

    var clientInfo = _clientConnected.GetClientInfo();

    try
    {
      while (!cts.IsCancellationRequested)
      {
        if (clientInfo is null)
        {
          clientInfo = _clientConnected.GetClientInfo();
          await Task.Delay(2000, cts);
          continue;
        }

        if (!CommunicationStateReceiveAndSend.IsConnected)
        {
          _logger.LogCritical("Remote connection has been lost, entering the reconnection mode!.");

          clientInfo.SocketWrapper?.InnerSocket!.Close();

          await ReconnectAsync((uint)clientInfo.SocketWrapper!.PortRemote, cts);
        }

        await Task.Delay(5000, cts);
      }
    }
    catch (Exception ex)
    {
      _logger.LogCritical(ex, "An error occurred when tryin" +
                              "g to reconnect with the remote!. ClientInfo: {ClientInfo}", clientInfo);
      throw new Exception();
    }
  }

  private async Task ReconnectAuthAsync(CancellationToken cts)
  {
    var objSocketSslStream = _clientConnected.GetObjSocketSslStream();

    try
    {
      while (!cts.IsCancellationRequested)
      {
        if (objSocketSslStream?.SocketWrapper is null)
        {
          objSocketSslStream = _clientConnected.GetObjSocketSslStream();
          await Task.Delay(2000, cts);
          continue;
        }

        if (!CommunicationStateReceiveAndSend.IsAuthenticated) continue;

        _logger.LogCritical("Remote authentication has been lost, entering the reconnection mode!.");

        var port = objSocketSslStream.SocketWrapper.PortRemote;

        objSocketSslStream.Disconnect();
   
        CommunicationStateReceiveAndSend.SetAuthenticated(false);

        await ReconnectAsync((uint)port, cts);
        await Task.Delay(5000, cts);
      }
    }
    catch (OperationCanceledException e)
    {
      logger.LogInformation("Operation to authenticate" +
                            " with Remote to listen to a timeout. Message: {Message}", e.Message);
      await ReconnectAuthAsync(cts);
    }
    catch (Exception e)
    {
      logger.LogCritical("An error occurred when " +
                            "trying to reconnect with the remote. Error: {Message}", e.Message);
      await ReconnectAuthAsync(cts);
      throw new Exception();
    }
  }

  private async Task ReconnectAsync(uint port, CancellationToken cts)
  {
    await _socketMiring.ReconnectAsync(port,
      TypeAuthMode.RequireAuthentication, cts);
  }
}
