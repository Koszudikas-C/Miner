using LibCommunicationStatusRemote.Entities;
using LibCommunicationStatusRemote.Entities.Enum;
using LibEntitiesRemote.Interface;

namespace LibCommunicationStateRemote.Entities;

public static class ClientAuthState
{
  public static Guid Id { get; private set; }
  private static IAuthDisconnectClient? ClientInfo { get; set; }
  private static AuthStateEnum StateOperations { get; set; }

  private static ClientAuthStateManager ClientAuthStateManager { get; set; } = new();

  public static void AddClientToAuthState(IAuthDisconnectClient clientInfo)
  {
    if (!clientInfo.Connected)
      throw new InvalidOperationException("The client was not connected." +
                                          " Check that it has been connected before adding authentication monitoring");

    Id = Guid.NewGuid();
    ClientInfo = clientInfo;
    StateOperations = AuthStateEnum.Pending;

    AddClientAuthStateManager();
  }
  
  public static void UpdateClientAuthState(AuthStateEnum newState)
  {
    StateOperations = newState;
    UpdateClientAuthStateManager();
  }

  private static void AddClientAuthStateManager()
  {
    var clientOperation = new ClientOperation<IAuthDisconnectClient>(StateOperations);
    ClientAuthStateManager.AddClientAuthState(ClientInfo!.Id, clientOperation);
  }
  
  private static void UpdateClientAuthStateManager()
  {
    ClientAuthStateManager.UpdateClientAuthState(ClientInfo!.Id, StateOperations);
  }
}
