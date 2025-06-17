using LibCommunicationStateClient.Entities.Enum;
using LibHandlerClient.Entities;

namespace LibException;

public class ReconnectException : Exception
{
    private readonly GlobalEventBus _globalEventBus = GlobalEventBus.Instance;
    private int Attempt { get; set; }
    public ReconnectException(string message, ConnectionStates connectionStates, CancellationToken cts = default)
        : base(message)
    {
        ReconnectAsync(connectionStates, cts).GetAwaiter();
        
    }

    public ReconnectException(string message, ConnectionStates connectionStates,
        Exception inner, CancellationToken cts = default) : base(message, inner)
    {
        ReconnectAsync(connectionStates, cts).GetAwaiter();
    }

    private async Task  ReconnectAsync(ConnectionStates connectionStates, CancellationToken cts = default)
    {
        
        await _globalEventBus.PublishAsync(connectionStates, cts);
    }
}