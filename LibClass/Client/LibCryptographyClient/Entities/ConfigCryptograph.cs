using System.Text;
using System.Text.Json;

namespace LibCryptographyClient.Entities;

public class ConfigCryptograph
{
    private const string KeyDefault = "22FFD66E1E314CCCA24CC78ACB5CC07C";
    private const string KeyHmacDefault = "19D32BABAEB042B7837AF8CB62EED569";

    
    private static readonly List<ConfigCryptograph> X1 = [];
    private static readonly object _lock = new();
    public byte[] HeaderSignature { get; set; } = [0x16, 0x34, 0x50, 0x75];
    public string Key { get; set; } = KeyDefault;
    public string HmacKey { get; set; } = KeyHmacDefault;
    private byte[]? EncryptKey { get; set; }
    public string FilePath { get; set; }
    private object? Data { get; set; }
    private byte[]? DataBytes { get; set; }
    
    public ConfigCryptograph(string filePath)
    {
        FilePath = filePath;
        
        AddOrUpdateConfigCryptograph(this);
    }

    public byte[] GetHmacKey()
    {
        if (string.IsNullOrWhiteSpace(HmacKey))
            throw new InvalidOperationException("Hmac key (HmacKey) must not be null or empty.");
        if (HmacKey.Length > 64)
            throw new InvalidOperationException("Hmac key (HmacKey) must be at least 64 characters.");
        return Encoding.UTF8.GetBytes(HmacKey);
    }

    public byte[] GetEncryptKey()
    {
        if (string.IsNullOrWhiteSpace(Key))
            throw new InvalidOperationException("Encryption key (Key) must not be null or empty.");
        if (Key.Length > 32)
            throw new InvalidOperationException("Encryption key (Key) must be at least 32 characters for AES.");
        return EncryptKey ??= Encoding.UTF8.GetBytes(Key);
    }

    public object GetData()
    {
        if (Data is null)
            throw new NullReferenceException("Data is null. Set data before using GetData().");
        return Data;
    }

    public void SetData(object data)
    {
        if (data is null)
            throw new ArgumentNullException(nameof(data), "Data cannot be null.");
        
        Data = data;
        DataBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
    }

    public void SetDataClear()
    {
        Data = null;
    }

    public byte[] GetDataBytes()
    {
        if (DataBytes is null)
            throw new InvalidOperationException(
                "DataBytes is null. Set data or data bytes before using GetDataBytes().");
        return DataBytes;
    }

    public void SetDataBytes(byte[] data)
    {
        if (data is null || data.Length == 0)
            throw new ArgumentException("DataBytes cannot be null or empty.", nameof(data));
        DataBytes = data;
    }

    public void AddOrUpdateConfigCryptograph(ConfigCryptograph configCryptograph)
    {
        lock (_lock)
        {
            var idx = X1.FindIndex(c => c.GetType() == typeof(ConfigCryptograph));
            if (idx >= 0)
            {
                X1[idx] = configCryptograph;
                return;
            }

            X1.Add(configCryptograph);
        }
    }

    public ConfigCryptograph GetConfigCryptograph(Type type)
    {
        lock (_lock)
        {
            if (X1.Any(c => c.GetType() == type))
                return X1.FirstOrDefault(c => c.GetType() == type)!;
        }

        throw new InvalidOperationException($"Cryptograph for type {type.Name} does not exist.");
    }

    public void RemoveConfigCryptograph(Type type)
    {
        lock (_lock)
        {
            X1.RemoveAll(c => c.GetType() == type);
            ClearSensitiveData();
        }
    }

    private void ClearSensitiveData()
    {
        if (EncryptKey is not null)
            Array.Clear(EncryptKey, 0, EncryptKey.Length);
        if (DataBytes is not null)
            Array.Clear(DataBytes, 0, DataBytes.Length);

        Array.Clear(HeaderSignature, 0, HeaderSignature.Length);
    }

    public static void ClearAllInstances()
    {
        lock (_lock)
        {
            foreach (var item in X1)
                item.ClearSensitiveData();
            X1.Clear();
        }
    }

    public void ValidateAll()
    {
        if (string.IsNullOrWhiteSpace(FilePath))
            throw new InvalidOperationException("FilePath must not be null or empty.");
        if (HeaderSignature is null || HeaderSignature.Length < 4)
            throw new InvalidOperationException("HeaderSignature must be at least 4 bytes.");
        if (string.IsNullOrWhiteSpace(Key) || Key.Length < 16)
            throw new InvalidOperationException("Key must not be null and must be at least 16 characters.");
        if ((Data is null) && (DataBytes is null))
            throw new InvalidOperationException("Either Data or DataBytes must be set.");
    }
}
