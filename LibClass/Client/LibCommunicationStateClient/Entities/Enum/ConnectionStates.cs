using System.Data;

namespace LibCommunicationStateClient.Entities.Enum;

public enum ConnectionStates
{
    Disconnected,
    Connecting,
    Connected,
    Authenticated,
    NoAuthenticated,
    Reconnecting,
    Faulted,
    
}