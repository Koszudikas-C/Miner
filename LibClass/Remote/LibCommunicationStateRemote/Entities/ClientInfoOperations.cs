using LibCommunicationStatusRemote.Entities.Enum;
using LibEntitiesRemote.Interface;

namespace LibCommunicationStatusRemote.Entities;

public class ClientOperation<T>(AuthStateEnum authStateEnum)
{
  public T? ClientInfo { get; set; }
  public AuthStateEnum StateOperation { get; set; }
}
