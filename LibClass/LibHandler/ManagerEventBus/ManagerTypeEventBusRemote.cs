using System.Net.Security;
using System.Text.Json;
using LibCryptography.Entities;
using LibDto.Dto;
using LibHandler.EventBus;
using LibJson.Util;
using LibManagerFile.Entities;
using LibRemoteAndClient.Entities.Client;
using LibRemoteAndClient.Entities.Remote.Client.Enum;
using LibSocketAndSslStream.Entities;

namespace LibHandler.ManagerEventBus;

public class ManagerTypeEventBusRemote
{
    private readonly GlobalEventBusRemote _globalEventBusRemote = GlobalEventBusRemote.Instance!;

    public void PublishEventType(JsonElement listData)
    {
        var obj = JsonElementConvertRemote.ConvertToObject(listData) ?? 
        throw new ArgumentNullException(nameof(listData));

        switch (obj)
        {
            case ClientMine clientMine:
                _globalEventBusRemote.Publish(clientMine);
                break;
            case LogEntry logEntry:
                _globalEventBusRemote.Publish(logEntry);
                break;
            case SslStream sslStream:
                _globalEventBusRemote.Publish(sslStream);
                break;
            case string message:
                _globalEventBusRemote.Publish(message);
                break;
            case GuidTokenAuth guidTokenAuth:
                _globalEventBusRemote.Publish(guidTokenAuth);
                break;
            case ClientCommandXmrig xmrigResult:
                _globalEventBusRemote.Publish(xmrigResult);
                break;
            case ClientCommandLog clientCommandLog:
                _globalEventBusRemote.Publish(clientCommandLog);
                break;
            case ConfigSaveFile configSaveFile:
                _globalEventBusRemote.Publish(configSaveFile);
                break;
            case ConfigCryptograph configCryptograph:
                _globalEventBusRemote.Publish(configCryptograph);
                break;
            case ConfigCryptographDto configCryptographDto:
                _globalEventBusRemote.Publish(configCryptographDto);
                break;
            case ConfigVariableDto configVariableDto:
                _globalEventBusRemote.Publish(configVariableDto);
                break;
            case ConfigSaveFileDto configSaveFileDto:
                _globalEventBusRemote.Publish(configSaveFileDto);
                break;
            default:
                throw new ArgumentException($"Unsupported listData type: {obj.GetType().FullName ?? "null"}", nameof(listData));
        }
    }

    public void PublishListEventType(List<JsonElement> listData)
    {
        if (listData == null || listData.Count == 0)
            throw new ArgumentNullException(nameof(listData), "List cannot be null or empty");

        var obj = listData.Select(JsonElementConvertRemote.ConvertToObject).ToList();

        if (obj == null || obj.Count == 0)
            throw new ArgumentNullException(nameof(listData));
        Console.WriteLine($"Type List: {obj.GetType().Name}");

        var firstType = obj.FirstOrDefault()?.GetType();
        if (firstType == null)
            throw new ArgumentException("Could not determine type of list elements.", nameof(listData));

        if (obj.All(o => o is ClientMine))
            _globalEventBusRemote.PublishList(obj.Cast<ClientMine>().ToList());
        else if (obj.All(o => o is LogEntry))
            _globalEventBusRemote.PublishList(obj.Cast<LogEntry>().ToList());
        else if (obj.All(o => o is string))
            _globalEventBusRemote.PublishList(obj.Cast<string>().ToList());
        else
        {
            var types = string.Join(", ", obj.Select(o => o?.GetType().FullName ?? "null").Distinct());
            throw new ArgumentException($"Unsupported listData list type(s): {types}", nameof(listData));
        }
    }
}