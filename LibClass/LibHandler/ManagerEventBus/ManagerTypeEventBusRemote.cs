using System.Net.Security;
using System.Text.Json;
using LibHandler.EventBus;
using LibJson.Util;
using LibRemoteAndClient.Entities.Client;
using LibRemoteAndClient.Entities.Remote.Client.Enum;

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
            default:
                throw new ArgumentException("Unsupported listData type", nameof(listData));
        }
    }

    public void PublishListEventType(List<JsonElement> listData)
    {
        var obj = listData.Select(JsonElementConvertRemote.ConvertToObject).ToList();

        if (obj == null || obj.Count == 0) throw new ArgumentNullException(nameof(listData));
        Console.WriteLine($"Type List: {obj.GetType().Name}");

        if (obj.All(o => o is ClientMine))
            _globalEventBusRemote.PublishList(obj.Cast<ClientMine>().ToList());
        else if (obj.All(o => o is LogEntry))
            _globalEventBusRemote.PublishList(obj.Cast<LogEntry>().ToList());
        else if (obj.All(o => o is string))
            _globalEventBusRemote.PublishList(obj.Cast<string>().ToList());
        else
            throw new ArgumentException("Unsupported listData list type", nameof(listData));
    }
}