using System.Reflection;
using System.Text.Json;
using LibRemoteAndClient.Entities.Client;
using LibRemoteAndClient.Entities.Client.Enum;

namespace LibJson.Util;

public class JsonElementConvertClient
{
    public static object ConvertToObject(JsonElement jsonElement)
    {
        return IdentifierTypeToProcesss(jsonElement);
    }

    private static object IdentifierTypeToProcesss(JsonElement jsonElement)
    {
        if (JsonMatchesType<ClientMine>(jsonElement))
            return jsonElement.Deserialize<ClientMine>()!;

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

        if (JsonValueKind.Object == jsonElement.ValueKind)
        {
            CreateOrUpdateConfigJson(jsonElement);

            return "";
        }

        if (jsonElement.ValueKind == JsonValueKind.String)
            return jsonElement.GetString()!;

        throw new ArgumentException("Unsupported data type", nameof(jsonElement));
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

        var formatted = JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions { WriteIndented = true });
        writer.Write(formatted);
    }
}
