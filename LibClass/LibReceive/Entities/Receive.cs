using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using LibCommunicationStatus;
using LibReceive.Entites;

namespace LibReceive.Entities;

public sealed class Receive(
    Socket socket,
    CancellationToken cancellationToken = default)
{
    private readonly Socket _socket = socket;
    private int _totalBytesReceived;
    private readonly CancellationToken _cancellationToken = cancellationToken;
    private readonly StateObject _buffer = new();
    public event Action<JsonElement>? OnReceivedAct;
    public event Action<List<JsonElement>>? OnReceivedListAct;
    public event Action<Socket>? OnClosedAct;

    public async Task ReceiveDataAsync(CancellationToken cancellationToken = default)
    {
        var token = MergeTokens(_cancellationToken, cancellationToken);

        await ExecuteWithTimeout(() => ReceiveLengthPrefixAsync(token), TimeSpan.FromSeconds(30), token);
        await ExecuteWithTimeout(() => ReceiveObjectAsync(token), TimeSpan.FromSeconds(30), token);

        DeserializeObject();
    }

    private async Task ReceiveLengthPrefixAsync(CancellationToken cancellationToken = default)
    {
        var received = 0;
        while (received < _buffer.BufferInit.Length)
        {
            var bytes = await _socket.ReceiveAsync(
                new ArraySegment<byte>(_buffer.BufferInit, received, _buffer.BufferInit.Length - received),
                SocketFlags.None, cancellationToken);

            if (bytes == 0) throw new SocketException();
            received += bytes;
        }

        _buffer.BufferSize = BitConverter.ToInt32(_buffer.BufferInit, 0);
        _buffer.IsList = _buffer.BufferInit[4] == 1;
        _buffer.BufferReceive = new byte[_buffer.BufferSize];
    }

    private async Task ReceiveObjectAsync(CancellationToken cancellationToken = default)
    {
        _totalBytesReceived = 0;
        while (_totalBytesReceived < _buffer.BufferSize)
        {
            var bytes = await _socket.ReceiveAsync(
                new ArraySegment<byte>(_buffer.BufferReceive, _totalBytesReceived, _buffer.BufferSize - _totalBytesReceived),
                SocketFlags.None, cancellationToken);

            if (bytes == 0) break;
            _totalBytesReceived += bytes;
        }
    }

    private void DeserializeObject()
    {
        if (_totalBytesReceived != _buffer.BufferSize) return;
        var jsonData = Encoding.UTF8.GetString(_buffer.BufferReceive, 0, _totalBytesReceived);

        if (_buffer.IsList)
        {
            var resultList = JsonSerializer.Deserialize<List<JsonElement>>(jsonData);
            OnReceivedList(resultList!);
            return;
        }

        var resultObj = JsonSerializer.Deserialize<JsonElement>(jsonData);
        OnReceived(resultObj!);
    }

    private async Task ExecuteWithTimeout(Func<Task> taskFunc, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        var timeoutTask = Task.Delay(timeout, cancellationToken);
        var task = taskFunc();

        if (await Task.WhenAny(task, timeoutTask) == timeoutTask)
        {
            OnClosed(_socket);
            CommunicationStatus.SetReceiving(false);
        }

        await task;
    }

    private void OnReceived(JsonElement data)
    {
        OnReceivedAct?.Invoke(data);
    }

    private void OnReceivedList(List<JsonElement> data)
    {
        OnReceivedListAct?.Invoke(data);
    }

    private void OnClosed(Socket socket) => OnClosedAct?.Invoke(socket);

    private static CancellationToken MergeTokens(CancellationToken original, CancellationToken external)
    {
        if (!original.CanBeCanceled && !external.CanBeCanceled)
            return CancellationToken.None;

        if (!original.CanBeCanceled) return external;
        if (!external.CanBeCanceled) return original;

        var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(original, external);
        return linkedSource.Token;
    }
}
