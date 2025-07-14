using System.Net.Sockets;
using LibEntitiesRemote.Entities.Client.Enum;

namespace LibSocketAndSslStreamRemote.Entities;

public class SocketsConnectedEvent(Socket socketClient, ConnectionStates states,
    int attempt, bool registered)
{
    public ConnectionStates ClientState { get; set; } = states;
    public int Attempts { get; set; } = attempt;
    public bool Registered { get; set; } = registered;
    public Socket SocketClient { get; set; } = socketClient;
}