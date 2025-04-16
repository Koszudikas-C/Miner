using System.Net.Security;
using System.Text;
using System.Text.Json;
using LibCommunicationStatus;
using LibReceive.Entites;

namespace LibReceive.Entities;

public sealed class ReceiveAuth(
    SslStream sslStream,
    CancellationToken cancellationToken = default)
{
    private readonly SslStream _sslStream = sslStream;
    private int _totalBytesReceived;
    private readonly CancellationToken _cancellationToken = cancellationToken;
    private readonly StateObject _buffer = new();
    public event Action<JsonElement>? OnReceivedAct;
    public event Action<List<JsonElement>>? OnReceivedListAct;
    public event Action<SslStream>? OnClosedAct;

    public async Task ReceiveDataAsync()
    {
        await ExecuteWithTimeout(ReceiveLengthPrefix, TimeSpan.FromMinutes(2));
        await ExecuteWithTimeout(ReceiveObject, TimeSpan.FromMinutes(2));

        DeserializeObject();
    }

    private async Task ReceiveLengthPrefix()
    {
        _ = await this._sslStream.ReadAsync(this._buffer.BufferInit, _cancellationToken);

        this._buffer.BufferSize = BitConverter.ToInt32(this._buffer.BufferInit, 0);
        this._buffer.IsList = this._buffer.BufferInit[4] == 1;

        this._buffer.BufferReceive = new byte[this._buffer.BufferSize];
    }

    private async Task ReceiveObject()
    {
        _totalBytesReceived = 0;
        while (_totalBytesReceived < this._buffer.BufferSize)
        {
            var bytesRead = await _sslStream.ReadAsync(
                this._buffer.BufferReceive.AsMemory(_totalBytesReceived,
                    this._buffer.BufferSize - _totalBytesReceived), _cancellationToken);

            if (bytesRead == 0) break;
            _totalBytesReceived += bytesRead;
        }
    }

    private void DeserializeObject()
    {
        if (this._totalBytesReceived != this._buffer.BufferSize) return;
        var jsonData = Encoding.UTF8.GetString(this._buffer.BufferReceive, 0, _totalBytesReceived);

        if (this._buffer.IsList)
        {
            var resultList = JsonSerializer.Deserialize<List<JsonElement>>(jsonData);
            OnReceivedList(resultList!);
            return;
        }

        var resultObj = JsonSerializer.Deserialize<JsonElement>(jsonData);
        OnReceived(resultObj!);
    }

    private async Task ExecuteWithTimeout(Func<Task> taskFunc, TimeSpan timeout)
    {
        var timeoutTask = Task.Delay(timeout, _cancellationToken);
        var task = taskFunc();

        if (await Task.WhenAny(task, timeoutTask) == timeoutTask)
        {
            OnClosed(_sslStream);
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
        OnReceivedListAct!.Invoke(data);
    }

    private void OnClosed(SslStream sslStream) => OnClosedAct?.Invoke(sslStream);
}