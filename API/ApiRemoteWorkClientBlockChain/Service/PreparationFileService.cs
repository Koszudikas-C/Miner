using LibPreparationFile.Interface;

namespace ApiRemoteWorkClientBlockChain.Service;

public class PreparationFileService(ILogger<PreparationFileService> logger) : IPreparationFile
{
    public string GetFileName(string path) => Path.GetFileName(path);

    public byte[] GetFileLengthByte(string path)
    {
        try
        {
            var file = new FileInfo(path);

            logger.LogInformation($"File found is successfully opened. path:{path}");
            return BitConverter.GetBytes(file.Length);
        }
        catch (Exception e)
        {
            logger.LogError(
                $"Proved the error is related to the path " +
                $"of the valid file in the DirectoryService service before calling this method. Error:{e.Message}");
            throw new Exception();
        }
    }

    public async Task<byte[]> GetFileByte(string path)
    {
        try
        {
            using var file = new FileStream(path, FileMode.Open);
            var buffer = new byte[8192];
            _ = await file.ReadAsync(buffer);

            return buffer;
        }
        catch (Exception)
        {
            throw new Exception();
        }
    }
}