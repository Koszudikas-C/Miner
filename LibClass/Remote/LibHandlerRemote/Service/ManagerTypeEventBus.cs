using System.Net;
using System.Text.Json;
using LibDtoRemote.Dto;
using LibDtoRemote.Dto.ClientMine;
using LibDtoRemote.Dto.Enum;
using LibHandlerRemote.Entities;
using LibUtilRemote.Util;

namespace LibHandlerRemote.Service;

public class ManagerTypeEventBus
{
    private readonly GlobalEventBus _globalEventBus = GlobalEventBus.Instance;

    public void PublishEventType(JsonElement data)
    {
        var obj = JsonElementConvert.ConvertToObject(data) ?? 
        throw new ArgumentNullException(nameof(data));

        switch (obj)
        {
            case LogEntryDto logEntry:
                _globalEventBus.Publish(logEntry);
                break;
            case string message:
                _globalEventBus.Publish(message);
                break;
            case GuidTokenAuthDto guidTokenAuth:
                _globalEventBus.Publish(guidTokenAuth);
                break;
            case ClientCommandXmrigDto xmrigResult:
                _globalEventBus.Publish(xmrigResult);
                break;
            case ClientCommandLogDto clientCommandLog:
                _globalEventBus.Publish(clientCommandLog);
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
            case HttpStatusCode httpStatusCode:
                _globalEventBus.Publish(httpStatusCode);
                break;
            case ClientHandshakeDto clientHandshakeDto:
                _globalEventBus.Publish(clientHandshakeDto);
                break;
            default:
                throw new ArgumentException($"Unsupported data type from publish: {obj.GetType().FullName ?? "null"}", nameof(data));
        }
    }

    public void PublishListEventType(List<JsonElement> listData)
    {
        if (listData == null || listData.Count == 0)
            throw new ArgumentNullException(nameof(listData), "List cannot be null or empty");

        var obj = listData.Select(JsonElementConvert.ConvertToObject).ToList();

        if (obj == null || obj.Count == 0)
            throw new ArgumentNullException(nameof(listData));
        Console.WriteLine($"Type List: {obj.GetType().Name}");

        var firstType = obj.FirstOrDefault()?.GetType();
        if (firstType == null)
            throw new ArgumentException("Could not determine type of list elements.", nameof(listData));

        if (obj.All(o => o is ClientMineDto))
            _globalEventBus.PublishList(obj.Cast<ClientMineDto>().ToList());
        else if (obj.All(o => o is LogEntryDto))
            _globalEventBus.PublishList(obj.Cast<LogEntryDto>().ToList());
        else if (obj.All(o => o is string))
            _globalEventBus.PublishList(obj.Cast<string>().ToList());
        else
        {
            var types = string.Join(", ", obj.Select(o => o.GetType().FullName ?? "null").Distinct());
            throw new ArgumentException($"Unsupported listData list type(s): {types}", nameof(listData));
        }
    }
}
