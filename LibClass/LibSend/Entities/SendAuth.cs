using System.Net.Security;
using System.Text;
using System.Text.Json;
using LibRemoteAndClient.Util;

namespace LibSend.Entities;

public class SendAuth<T>(SslStream sslStream)
{
    private readonly SslStream _sslStream = sslStream;
    private readonly StateObject _buffer = new();
    public event Action<T>? SendingAct;
    public event Action<T>? SentAct;
    public event Action<List<T>>? SentListAct;

    public async Task SendAsync(T data, CancellationToken cts = default)
    {
        await ExecuteWithTimeout(() => SendLengthPrefix(data!, false, cts), TimeSpan.FromSeconds(30), cts);
        await ExecuteWithTimeout(() => SendObject(data!, cts), TimeSpan.FromSeconds(30), cts);

        await _sslStream.FlushAsync(cts);
    }

    public async Task SendListAsync(List<T> listData,
        CancellationToken cts = default)
    {
        await SendLengthPrefix(listData, true, cts);
        await SendObject(listData, cts);

        await _sslStream.FlushAsync(cts);
    }

    public async Task SendFileAsync(T fileData, CancellationToken cts = default)
    {
        var fileDataBytes = await SendLengthPrefixFile(fileData, cts);
        await SendFileObj(fileDataBytes, cts);
        
        await _sslStream.FlushAsync(cts);
    }

    private async Task<byte[]> SendLengthPrefixFile(T fileData, CancellationToken cts = default)
    {
        try
        {
            var fileDataBytes = Encoding.UTF8.GetBytes(fileData!.ToString()!);
            _buffer.BufferSend = BitConverter.GetBytes(fileDataBytes.Length);

            await _sslStream.WriteAsync(_buffer.BufferSend, cts);

            return fileDataBytes;
        }
        catch (Exception)
        {
            throw new Exception("Error when sending the size of the object.");
        }
    }

    private async Task SendFileObj(byte[] fileData, CancellationToken cts = default)
    {
        try
        {
            _buffer.BufferInit = new byte[81920];
            var offset = 0;
            while (offset < fileData.Length)
            {
                var chunkSize  = Math.Min(_buffer.BufferInit.Length, fileData.Length - offset);
                await _sslStream.WriteAsync(fileData.AsMemory(0, chunkSize), cts);
                offset += chunkSize;
            }
        }
        catch (Exception e)
        {
            throw new Exception($"Error by sending the OBJ from the file. Error: {e.Message}");
        }
    }

    private async Task SendLengthPrefix(object data,
        bool isList = false, CancellationToken cts = default)
    {
        try
        {
            _buffer.BufferSend = BitConverter.GetBytes(JsonSerializer.SerializeToUtf8Bytes(data).Length);

            Array.Copy(this._buffer.BufferSend, this._buffer.BufferInit, 4);
            _buffer.BufferInit[4] = isList ? (byte)1 : (byte)0;

            await _sslStream.WriteAsync(this._buffer.BufferInit, cts);
            CheckWhichType(data, isList);
        }
        catch (Exception)
        {
            throw new Exception("Error when sending the size of the object.");
        }
    }

    private async Task SendObject(object data, CancellationToken cts = default)
    {
        this._buffer.BufferSend = JsonSerializer.SerializeToUtf8Bytes(data);
        await _sslStream.WriteAsync(this._buffer.BufferSend, cts);

        if (data is List<T> listData)
        {
            SentListAct?.Invoke(listData);
            return;
        }

        SentAct?.Invoke((T)data);
    }

    private static async Task ExecuteWithTimeout(Func<Task> taskFunc,
        TimeSpan timeout, CancellationToken cts = default)
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