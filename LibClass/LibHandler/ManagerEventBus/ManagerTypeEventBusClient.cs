using System.Net;
using System.Text.Json;
using LibHandler.EventBus;
using LibJson.Util;
using LibRemoteAndClient.Entities.Client.Enum;
using LibRemoteAndClient.Entities.Client;

namespace LibHandler.ManagerEventBus;

public class ManagerTypeEventBusClient : ManagerTypeEventBusBase
{
    private readonly GlobalEventBusClient _globalEventBus = GlobalEventBusClient.Instance!;
    
    public override void PublishEventType(JsonElement data)
    {
        var obj = JsonElementConvertClient.ConvertToObject(data) ??
                  throw new ArgumentNullException(nameof(data));

        switch (obj)
        {
            case ClientMine clientMine:
                _globalEventBus.Publish(clientMine);
                break;
            // case ListenerClient listener:
            //     _globalEventBus.Publish(listener);
            //     break;
            case LogEntry logEntry:
                _globalEventBus.Publish(logEntry);
                break;
            case ClientCommandMine clientCommandMine:
                _globalEventBus.Publish(clientCommandMine);
                break;
            case ClientCommandLog clientCommandLog:
                _globalEventBus.Publish(clientCommandLog);
                break;
            case HttpStatusCode httpStatusCode:
                _globalEventBus.Publish(httpStatusCode);
                break;
            case string message:
                _globalEventBus.Publish(message);
                break;
            default:
                throw new ArgumentException("Unsupported data type", nameof(data));
        }
    }

    public override void PublishListEventType(List<JsonElement> listData)
    {
        var obj = listData.Select(JsonElementConvertClient.ConvertToObject).ToList();

        if (obj.All(o => o is ClientMine))
        {
            _globalEventBus.Publish(obj.Cast<ClientMine>().ToList());
        }
        else if(obj.All(o => o is LogEntry))
        {
            _globalEventBus.Publish(obj.Cast<LogEntry>().ToList());
        }
        else if(obj.All(o => o is ClientCommandMine))
        {
            _globalEventBus.Publish(obj.Cast<ClientCommandMine>().ToList());
        }
    }
}