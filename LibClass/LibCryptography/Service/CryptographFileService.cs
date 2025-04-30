using System.Security.Cryptography;
using System.Text;
using LibCryptography.Entities;
using LibCryptography.Interface;

namespace LibCryptography.Service;

public class CryptographFileService : ICryptographFile
{
    public byte[] SaveFile(ConfigCryptograph configCrpytograph)
    {
        if (IsFileEncrypted(configCrpytograph.FilePath!, configCrpytograph.HeaderSignature))
            throw new CryptographicException("File is already encrypted");

        var encryptedData = Encrypt(configCrpytograph.GetEncryptKey()!,
            configCrpytograph.GetDataBytes(),
            configCrpytograph.HeaderSignature);

        var hmac = EncryptHmac(configCrpytograph.GetHmacKey()!, encryptedData);

        var result = encryptedData.Concat(hmac).ToArray();
        File.WriteAllBytes(configCrpytograph.FilePath!, result);

        return result;
    }
    
    public string LoadFile(ConfigCryptograph configCrpytograph)
    {
        if (!IsFileEncrypted(configCrpytograph.FilePath, configCrpytograph.HeaderSignature)) 
            throw new CryptographicException();
        
        var decryptHmac = DecryptHmac(configCrpytograph.GetHmacKey(),
            configCrpytograph.GetDataBytes()!);
        
        var data = Decrypt(configCrpytograph.GetEncryptKey()!,
            configCrpytograph.HeaderSignature, decryptHmac);
        
        return data;
    }

    private static byte[] Encrypt(byte[] key, byte[] plainVBytes, 
        byte[] headerSignature)
    {
        using var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Key = key;
        
        using var encryptor = aes.CreateEncryptor();
        var encryptedBytes = encryptor.TransformFinalBlock(plainVBytes,
            0, plainVBytes.Length);
        
        return [.. headerSignature, .. aes.IV, .. encryptedBytes];
    }

    private static string Decrypt(byte[] key, byte[] headerSignature, byte[] encryptedData)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.Mode = CipherMode.CBC;
        
        aes.IV = [.. encryptedData.Skip(headerSignature.Length).Take(16)];
        var cipherBytes = encryptedData.Skip(headerSignature.Length + 16).ToArray();

        using var decrypted = aes.CreateDecryptor();
        var plainBytes = decrypted.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }

    private static byte[] EncryptHmac(byte[] key, byte[] data)
    {
        if(data is null) throw new ArgumentNullException(nameof(data), "Data cannot be null.");
        
        using var hmac = new HMACSHA512(key);
        var hmacHash = hmac.ComputeHash(data);
        
        return hmacHash;
    }
    
    private static byte[] DecryptHmac(byte[] key, byte[] data)
    {
        if(data is null) throw new ArgumentNullException(nameof(data), "Data cannot be null.");
        
        var originalData = data.Take(data.Length - 64).ToArray();
        var receivedHmac = data.Skip(data.Length - 64).ToArray();

        using var hmac = new HMACSHA512(key);
        var calculatedHmac = hmac.ComputeHash(originalData);

        if (!calculatedHmac.SequenceEqual(receivedHmac))
            throw new CryptographicException("HMAC validation failed. Data may have been tampered.");

        return originalData;
    }
    
    private static bool IsFileEncrypted(string filePath, byte[] headerSignature)
    {
        var buffer = new byte[headerSignature.Length];
        using var fs = File.OpenRead(filePath);
        var read = fs.Read(buffer, 0, headerSignature.Length);
        return read == headerSignature.Length && buffer.SequenceEqual(headerSignature);
    }
}