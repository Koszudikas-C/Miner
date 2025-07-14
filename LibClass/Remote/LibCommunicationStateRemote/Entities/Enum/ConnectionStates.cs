using System.Data;

namespace LibCommunicationStateRemote.Entities.Enum;

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