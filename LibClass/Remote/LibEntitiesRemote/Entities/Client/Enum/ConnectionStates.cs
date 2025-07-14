using System.Data;

namespace LibEntitiesRemote.Entities.Client.Enum;

public enum ConnectionStates
{
    Disconnected,
    Connecting,
    Connected,
    Authenticated,
    NoAuthenticated,
    Reconnecting,
    Faulted,
    Blocked,
    Attempt,
    
}