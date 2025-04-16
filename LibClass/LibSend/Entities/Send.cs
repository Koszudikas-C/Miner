using System.Net.Sockets;
using System.Text.Json;
using LibRemoteAndClient.Util;

namespace LibSend.Entities;

public class Send<T>(Socket socket)
{
    private readonly Socket _socket = socket;
    private readonly StateObject _buffer = new();
    public event Action<T>? SendingAct;
    public event Action<T>? SentAct;
    public event Action<List<T>>? SentListAct;

    public async Task SendAsync(T data, CancellationToken cts = default)
    {
        await ExecuteWithTimeout(() => SendLengthPrefix(data!, false, cts), TimeSpan.FromMinutes(1), cts);
        await ExecuteWithTimeout(() => SendObject(data!, cts), TimeSpan.FromMinutes(1), cts);
    }

    public async Task SendListAsync(List<T> listData, CancellationToken cts = default)
    {
        await SendLengthPrefix(listData, true, cts);
        await SendObject(listData, cts);
    }

    private async Task SendLengthPrefix(object data, bool isList = false, CancellationToken cts = default)
    {
        _buffer.BufferSend = BitConverter.GetBytes(JsonSerializer.SerializeToUtf8Bytes(data).Length);

        Array.Copy(_buffer.BufferSend, _buffer.BufferInit, 4);
        _buffer.BufferInit[4] = isList ? (byte)1 : (byte)0;

        await _socket.SendAsync(_buffer.BufferInit.AsMemory(0, 5), SocketFlags.None, cts);
        CheckWhichType(data, isList);
    }

    private async Task SendObject(object data, CancellationToken cts = default)
    {
        _buffer.BufferSend = JsonSerializer.SerializeToUtf8Bytes(data);
        await _socket.SendAsync(_buffer.BufferSend, SocketFlags.None, cts);

        if (data is List<T> listData)
        {
            SentListAct?.Invoke(listData);
            return;
        }

        SentAct?.Invoke((T)data);
    }

    private static async Task ExecuteWithTimeout(Func<Task> taskFunc, TimeSpan timeout, CancellationToken cts = default)
    {
        var timeoutTask = Task.Delay(timeout, cts);
        var task = taskFunc();

        if (await Task.WhenAny(task, timeoutTask) == timeoutTask)
            throw new TimeoutException("Operation timed out.");

        await task;
    }

    private void CheckWhichType(object data, bool isList)
    {
        if (isList)
        {
            SentListAct?.Invoke((List<T>)data);
            return;
        }

        OnSending((T)data);
    }

    private void OnSending(T data)
    {
        SendingAct?.Invoke(data);
    }
}
