using System.Net;
using System.Text.Json;
using LibDtoClient.Dto;
using LibDtoClient.Dto.ClientMine;
using LibDtoClient.Dto.Enum;
using LibHandlerClient.Entities;
using LibUtilClient.Util;

namespace LibHandlerClient.Service;

public class ManagerTypeEventBus : ManagerTypeEventBusBase
{
    private readonly GlobalEventBus _globalEventBus = GlobalEventBus.Instance;

    public override void PublishEventType(JsonElement data)
    {
        var obj = JsonElementConvert.ConvertToObject(data) ??
                  throw new ArgumentNullException(nameof(data));

        switch (obj)
        {
            case LogEntryDto logEntry:
                _globalEventBus.Publish(logEntry);
                break;
            case ClientCommandMineDto clientCommandMine:
                _globalEventBus.Publish(clientCommandMine);
                break;
            case ClientCommandLogDto clientCommandLog:
                _globalEventBus.Publish(clientCommandLog);
                break;
            case HttpStatusCode httpStatusCode:
                _globalEventBus.Publish(httpStatusCode);
                break;
            case string message:
                _globalEventBus.Publish(message);
                break;
            case ConfigSaveFileDto configSaveFile:
                _globalEventBus.Publish(configSaveFile);
                break;
            case ConfigCryptographDto configCryptograph:
                _globalEventBus.Publish(configCryptograph);
                break;
            case ConfigVariableDto configVariableDto:
                _globalEventBus.Publish(configVariableDto);
                break;
            case ClientMineDto clientMineDto:
                _globalEventBus.Publish(clientMineDto);
                break;
            case ParamsManagerOptionsDto<ParamsSocks5Dto> paramsManagerOptionsDto:
                _globalEventBus.Publish(paramsManagerOptionsDto);
                break;
            case ParamsSocks5Dto paramsSocks5Dto:
                _globalEventBus.Publish(paramsSocks5Dto);
                break;
            case DownloadRequestDto downloadRequestDto:
                _globalEventBus.Publish(downloadRequestDto);
                break;
            case UploadResponseHeaderDto uploadResponseHeaderDto:
                _globalEventBus.Publish(uploadResponseHeaderDto);
                break;
            case UploadResponseDto uploadResponseDto:
                _globalEventBus.Publish(uploadResponseDto);
                break;
            case ParamsManagerOptionsResponseDto paramsManagerOptionsResponseDto:
                _globalEventBus.Publish(paramsManagerOptionsResponseDto);
                break;
            case GuidTokenAuthDto guidTokenAuth:
                _globalEventBus.Publish(guidTokenAuth);
                break;
            case ClientHandshakeDto clientHandshakeDto:
                _globalEventBus.Publish(clientHandshakeDto);
                break;
            default:
                throw new ArgumentException($"Unsupported data type from publish: {obj.GetType().FullName ?? "null"}", nameof(data));
        }
    }

    public override void PublishListEventType(List<JsonElement> listData)
    {
        if (listData == null || listData.Count == 0)
            throw new ArgumentNullException(nameof(listData), "List cannot be null or empty");

        var obj = listData.Select(JsonElementConvert.ConvertToObject).ToList();

        var firstType = obj.FirstOrDefault()?.GetType();
        if (firstType == null)
            throw new ArgumentException("Could not determine type of list elements.", nameof(listData));

        if (obj.All(o => o is ClientMineDto))
        {
            _globalEventBus.Publish(obj.Cast<ClientMineDto>().ToList());
        }
        else if (obj.All(o => o is LogEntryDto))
        {
            _globalEventBus.Publish(obj.Cast<LogEntryDto>().ToList());
        }
        else if (obj.All(o => o is ClientCommandMineDto))
        {
            _globalEventBus.Publish(obj.Cast<ClientCommandMineDto>().ToList());
        }
        else
        {
            var types = string.Join(", ", obj.Select(o => o.GetType().FullName ?? "null").Distinct());
            throw new ArgumentException($"Unsupported list type(s): {types}", nameof(listData));
        }
    }
}
