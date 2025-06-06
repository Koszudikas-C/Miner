using System.Net.Security;
using System.Text;
using System.Text.Json;
using LibCommunicationStatusRemote.Entities;
using LibReceive.Entites;

namespace LibReceiveRemote.Entities;

public sealed class ReceiveAuth(SslStream sslStream)
{
    private readonly SslStream _sslStream = sslStream;
    private int _totalBytesReceived;
    private readonly StateObject _buffer = new();
    public event Action<JsonElement>? OnReceivedAct;
    public event Action<List<JsonElement>>? OnReceivedListAct;
    public event Action<SslStream>? OnClosedAct;

    public async Task ReceiveDataAsync(CancellationToken cts = default)
    {
        await ExecuteWithTimeoutAsync(() => ReceiveLengthPrefixAsync(cts), TimeSpan.FromSeconds(30), cts);
        await ExecuteWithTimeoutAsync(() => ReceiveObjectAsync(cts), TimeSpan.FromSeconds(30), cts);
        
        await _sslStream.FlushAsync(cts);
        DeserializeObject();
    }

    public async Task ReceiveDataFileAsync(CancellationToken cts = default)
    {
        await ReceiveLengthPrefixAsync(cts);
        await ReceiveObjFileAsync(cts);
        await _sslStream.FlushAsync(cts);

        DeserializeFile();
    }

    private void DeserializeFile()
    {
        var result = Encoding.UTF8.GetString(_buffer.BufferReceive);
    }

    private async Task ReceiveObjFileAsync(CancellationToken cts = default)
    {
        try
        {
            using var ms = new MemoryStream();
            _buffer.BufferInit = new byte[81920];

            var remaining = _buffer.BufferReceive.Length;

            while (remaining > 0)
            {
                var readSize = Math.Min(_buffer.BufferInit.Length, remaining);
                _totalBytesReceived = await _sslStream.ReadAsync(_buffer.BufferInit.AsMemory(0, readSize), cts);
                if (_totalBytesReceived == 0) throw new IOException("Prematurely closed connection");

                await ms.WriteAsync(_buffer.BufferInit.AsMemory(0, _totalBytesReceived), cts);

                remaining -= _totalBytesReceived;
            }
        }
        catch (Exception)
        {
            throw new Exception("Error when receiving object file.");
        }
    }

    private async Task ReceiveLengthPrefixAsync(CancellationToken cts = default)
    {
        try
        {
            _ = await _sslStream.ReadAsync(_buffer.BufferInit, cts);

            _buffer.BufferSize = BitConverter.ToInt32(_buffer.BufferInit, 0);
            _buffer.IsList = _buffer.BufferInit[4] == 1;

            _buffer.BufferReceive = new byte[_buffer.BufferSize];
        }
        catch (Exception)
        {
            throw new Exception("Error sending object size.");
        }
    }

    private async Task ReceiveObjectAsync(CancellationToken cts = default)
    {
        try
        {
            _totalBytesReceived = 0;
            while (_totalBytesReceived < _buffer.BufferSize)
            {
                var bytesRead = await _sslStream.ReadAsync(
                    _buffer.BufferReceive.AsMemory(_totalBytesReceived,
                        _buffer.BufferSize - _totalBytesReceived), cts);

                if (bytesRead == 0) break;
                _totalBytesReceived += bytesRead;
            }
        }
        catch (Exception e)
        {
            throw new Exception("Error when sending the object.");
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

    private async Task ExecuteWithTimeoutAsync(Func<Task> taskFunc, TimeSpan timeout,
        CancellationToken cts = default)
    {
        var timeoutTask = Task.Delay(timeout, cts);
        var task = taskFunc();

        if (await Task.WhenAny(task, timeoutTask) == timeoutTask)
        {
            OnClosed(_sslStream);
            CommunicationStateReceiveAndSend.SetReceiving(false);
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
