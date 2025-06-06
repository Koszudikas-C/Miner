using System.Net.Sockets;
using LibCommunicationStateRemote.Entities;
using LibCommunicationStatusRemote.Entities.Enum;
using LibEntitiesRemote.Entities;
using LibHandlerRemote.Entities;
using LibReceiveRemote.Interface;
using LibSocketAndSslStreamRemote.Entities.Enum;
using LibSocketAndSslStreamRemote.Interface;

namespace LibSocketRemote.Service;

public class SocketService(
  IListener listenerRemote,
  IReceive receive) : ISocketMiring
{
  private readonly GlobalEventBus _globalEventBus = GlobalEventBus.Instance;

  public async Task InitializeAsync(uint port, int maxConnection, TypeAuthMode typeAuthMode,
    CancellationToken cts = default)
  {
    if (port is < 1000 or > 9999)
      throw new ArgumentException("Port number must be a 4-digit number between 1000 and 9999.");
    try
    {
      StartRemote(port, maxConnection, typeAuthMode);
      await Task.CompletedTask;
    }
    catch (Exception)
    {
      ClientAuthState.UpdateClientAuthState(AuthStateEnum.Failed);
      throw;
    }
  }

  private void StartRemote(uint port, int maxConnection,
    TypeAuthMode typeAuthMode)
  {
    listenerRemote.ConnectedAct += (socket) =>
      OnSocketConnectRemoteAuthAsync(socket, typeAuthMode);

    Task.Run(() => listenerRemote.StartAsync(typeAuthMode, port, maxConnection));
  }

  private void OnSocketConnectRemoteAuthAsync(Socket socket, TypeAuthMode typeAuthMode)
  {
    MapperTypeObj(socket, typeAuthMode);
  }

  public Task ReconnectAsync(uint port, int maxConnection,
    TypeAuthMode typeAuthMode, CancellationToken cts = default)
  {
    throw new NotImplementedException();
  }

  private void MapperTypeObj(Socket socket, TypeAuthMode typeAuthMode)
  {
    if (typeAuthMode == TypeAuthMode.AllowAnonymous)
    {
      var clientInfo = new ClientInfoAuth(socket);
      ClientAuthState.AddClientToAuthState(clientInfo);
      PublishTyped(clientInfo);
    }
    else
    {
      var objSocket = new ObjSocketSslStream(socket);
      ClientAuthState.AddClientToAuthState(objSocket);
      PublishTyped(objSocket);
    }
  }

  private static void DisconnectSocket(Socket socket) => socket.Close();

  private void PublishTyped<T>(T data)
  {
    _globalEventBus.Publish(data);
  }
}
