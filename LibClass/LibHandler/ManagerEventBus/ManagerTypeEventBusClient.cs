using System.Net;
using System.Text.Json;
using LibCryptography.Entities;
using LibDto.Dto;
using LibDto.Dto.ClientMine;
using LibHandler.EventBus;
using LibJson.Util;
using LibManagerFile.Entities;
using LibRemoteAndClient.Entities.Client.Enum;
using LibRemoteAndClient.Entities.Client;
using LibSocketAndSslStream.Entities;

namespace LibHandler.ManagerEventBus;

public class ManagerTypeEventBusClient : ManagerTypeEventBusBase
{
    private readonly GlobalEventBusClient _globalEventBusClient = GlobalEventBusClient.Instance!;
    
    public override void PublishEventType(JsonElement data)
    {
        var obj = JsonElementConvertClient.ConvertToObject(data) ??
                  throw new ArgumentNullException(nameof(data));

        switch (obj)
        {
            case LogEntry logEntry:
                _globalEventBusClient.Publish(logEntry);
                break;
            case ClientCommandMine clientCommandMine:
                _globalEventBusClient.Publish(clientCommandMine);
                break;
            case ClientCommandLog clientCommandLog:
                _globalEventBusClient.Publish(clientCommandLog);
                break;
            case HttpStatusCode httpStatusCode:
                _globalEventBusClient.Publish(httpStatusCode);
                break;
            case string message:
                _globalEventBusClient.Publish(message);
                break;
            case ConfigSaveFile configSaveFile:
                _globalEventBusClient.Publish(configSaveFile);
                break;
            case ConfigCryptograph configCryptograph:
                _globalEventBusClient.Publish(configCryptograph);
                break;
            case ConfigCryptographDto configCryptographDto:
                _globalEventBusClient.Publish(configCryptographDto);
                break;
            case ConfigVariableDto configVariableDto:
                _globalEventBusClient.Publish(configVariableDto);
                break;
            case ConfigSaveFileDto configSaveFileDto:
                _globalEventBusClient.Publish(configSaveFileDto);
                break;
            case ClientMineDto clientMineDto:
                _globalEventBusClient.Publish(clientMineDto); 
                break;
            default:
                throw new ArgumentException($"Unsupported data type: {obj.GetType().FullName ?? "null"}", nameof(data));
        }
    }

    public override void PublishListEventType(List<JsonElement> listData)
    {
        if (listData == null || listData.Count == 0)
            throw new ArgumentNullException(nameof(listData), "List cannot be null or empty");

        var obj = listData.Select(JsonElementConvertClient.ConvertToObject).ToList();

        var firstType = obj.FirstOrDefault()?.GetType();
        if (firstType == null)
            throw new ArgumentException("Could not determine type of list elements.", nameof(listData));

        if (obj.All(o => o is ClientMine))
        {
            _globalEventBusClient.Publish(obj.Cast<ClientMine>().ToList());
        }
        else if(obj.All(o => o is LogEntry))
        {
            _globalEventBusClient.Publish(obj.Cast<LogEntry>().ToList());
        }
        else if(obj.All(o => o is ClientCommandMine))
        {
            _globalEventBusClient.Publish(obj.Cast<ClientCommandMine>().ToList());
        }
        else
        {
            var types = string.Join(", ", obj.Select(o => o.GetType().FullName ?? "null").Distinct());
            throw new ArgumentException($"Unsupported list type(s): {types}", nameof(listData));
        }
    }
}