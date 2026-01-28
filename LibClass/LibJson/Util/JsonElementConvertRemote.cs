using System.Net;
using System.Reflection;
using System.Text.Json;
using LibDto.Dto;
using LibDto.Dto.ClientMine;
using LibRemoteAndClient.Entities.Client;
using LibRemoteAndClient.Entities.Remote.Client.Enum;
using LogEntry = LibRemoteAndClient.Entities.Remote.Client.LogEntry;

namespace LibJson.Util;

public static class JsonElementConvertRemote
{
    public static object ConvertToObject(JsonElement jsonElement)
    {
        return IdentifierTypeToProcesss(jsonElement);
    }

    private static object IdentifierTypeToProcesss(JsonElement jsonElement)
    {
        if (JsonMatchesType<LogEntry>(jsonElement))
            return jsonElement.Deserialize<LogEntry>()!;

        if (JsonMatchesType<ClientCommandMine>(jsonElement))
        {
            if (jsonElement.ValueKind == JsonValueKind.Number)
                return (ClientCommandMine)jsonElement.GetInt32();

            throw new InvalidOperationException("Expected a number for enum deserialization.");
        }

        if (JsonMatchesType<ClientCommandLog>(jsonElement))
        {
            if (jsonElement.ValueKind == JsonValueKind.Number)
                return (ClientCommandLog)jsonElement.GetInt32();

            throw new InvalidOperationException("Expected a number for enum deserialization.");
        }

        if (JsonMatchesType<GuidTokenAuth>(jsonElement))
            return jsonElement.Deserialize<GuidTokenAuth>()!;
        
        if (JsonMatchesType<HttpStatusCode>(jsonElement))
            return jsonElement.Deserialize<HttpStatusCode>();
        
        if(JsonMatchesType<ClientHandshakeDto>(jsonElement))
            return jsonElement.Deserialize<ClientHandshakeDto>()!;
        

        return IdentifierTypeToProcess1(jsonElement);
    }

    private static object IdentifierTypeToProcess1(JsonElement jsonElement)
    {
        if (JsonMatchesType<ConfigSaveFileDto>(jsonElement))
            return jsonElement.Deserialize<ConfigSaveFileDto>()!;

        if (JsonMatchesType<ConfigCryptographDto>(jsonElement))
            return jsonElement.Deserialize<ConfigCryptographDto>()!;

        if (JsonMatchesType<ConfigVariableDto>(jsonElement))
            return jsonElement.Deserialize<ConfigVariableDto>()!;

        if (JsonMatchesType<ClientMineDto>(jsonElement))
            return jsonElement.Deserialize<ClientMineDto>()!;

        if (JsonMatchesType<ParamsManagerOptionsDto<ParamsSocks5Dto>>(jsonElement))
            return jsonElement.Deserialize<ParamsManagerOptionsDto<ParamsSocks5Dto>>()!;

        if (JsonMatchesType<ParamsSocks5Dto>(jsonElement))
            return jsonElement.Deserialize<ParamsSocks5Dto>()!;

        if (JsonMatchesType<DownloadRequestDto>(jsonElement))
            return jsonElement.Deserialize<DownloadRequestDto>()!;
        
        if (JsonMatchesType<UploadResponseHeaderDto>(jsonElement))
            return jsonElement.Deserialize<UploadResponseHeaderDto>()!;

        if (JsonMatchesType<UploadResponseDto>(jsonElement))
            return jsonElement.Deserialize<UploadResponseDto>()!;

        if (JsonMatchesType<ParamsSocks5Dto>(jsonElement))
            return jsonElement.Deserialize<ParamsSocks5Dto>()!;
        
        if(JsonMatchesType<ParamsManagerOptionsResponseDto>(jsonElement))
            return jsonElement.Deserialize<ParamsManagerOptionsResponseDto>()!;

        if (JsonMatchesType<DownloadRequestDto>(jsonElement))
            return jsonElement.Deserialize<DownloadRequestDto>()!;

        if (jsonElement.ValueKind == JsonValueKind.Object)
        {
            CreateOrUpdateConfigJson(jsonElement);
            throw new InvalidOperationException(
                "Object type JSON Not recognized for conversion. No corresponding type found.");
        }

        if (jsonElement.ValueKind == JsonValueKind.String)
            return jsonElement.GetString()!;

        throw new ArgumentException("Unsupported data type json convert", nameof(jsonElement));
    }

    private static bool JsonMatchesType<T>(JsonElement element)
    {
        if (typeof(T).IsEnum)
        {
            return element.ValueKind switch
            {
                JsonValueKind.Number => Enum.IsDefined(typeof(T), element.GetInt32()),
                JsonValueKind.String => Enum.TryParse(typeof(T), element.GetString(), out _),
                _ => false
            };
        }

        if (element.ValueKind != JsonValueKind.Object)
        {
            return false;
        }

        var propertyJson = element.EnumerateObject().Select(p => p.Name).ToHashSet();
        var propertyClass = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name).ToHashSet();

        return propertyJson.SetEquals(propertyClass);
    }

    private static void CreateOrUpdateConfigJson(JsonElement jsonElement)
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        using var writer = new StreamWriter(stream);

        var formatted = JsonSerializer.Serialize(jsonElement,
            new JsonSerializerOptions { WriteIndented = true });

        writer.Write(formatted);
    }
}