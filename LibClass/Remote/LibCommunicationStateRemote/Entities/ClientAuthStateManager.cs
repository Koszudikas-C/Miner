using LibCommunicationStatusRemote.Entities.Enum;
using LibEntitiesRemote.Interface;

namespace LibCommunicationStatusRemote.Entities;

public class ClientAuthStateManager
{
  private readonly Dictionary<Guid, ClientOperation<IAuthDisconnectClient>> _clientsInfoAuthStates = [];

  public void AddClientAuthState(Guid id, ClientOperation<IAuthDisconnectClient> clientInfoOperation)
  {
    _clientsInfoAuthStates[id] = clientInfoOperation;
  }
  
  public void UpdateClientAuthState(Guid id, AuthStateEnum newState)
  {
    if (!_clientsInfoAuthStates.TryGetValue(id, out var clientInfoOperation)) return;

    CheckClientAuthState(clientInfoOperation);
    clientInfoOperation.StateOperation = newState;
  }

  public ClientOperation<IAuthDisconnectClient>? GetClientAuthState(Guid clientInfoId)
  {
    return _clientsInfoAuthStates.GetValueOrDefault(clientInfoId);
  }

  private static void CheckClientAuthState(ClientOperation<IAuthDisconnectClient> clientInfoOperation)
  {
    switch (clientInfoOperation.StateOperation)
    {
      case AuthStateEnum.Expired when clientInfoOperation.ClientInfo!.Connected:
      case AuthStateEnum.Failed when clientInfoOperation.ClientInfo!.Connected:
        clientInfoOperation.ClientInfo.Disconnect();
        break;
    }
  }
}
