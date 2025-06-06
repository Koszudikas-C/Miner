using System.Runtime.InteropServices;
using LibManagerFileClient.Entities;
using LibManagerFileClient.Interface;


namespace LibSaveFileClient.Service;

public class SaveFileService : ISaveFile
{
    private int _count;
    
    public async Task<string> SaveFileWriteAsync(ConfigSaveFile configSaveFile,
        CancellationToken cts = default)
    {
        try
        {
            configSaveFile.SetPathFile(PreparationConfigSaveFile(configSaveFile.PathFile));
            var writeTask = File.WriteAllTextAsync(configSaveFile.PathFile,
                configSaveFile.Data, cts);

            var completedTask = await Task.WhenAny(writeTask, Task.Delay(configSaveFile.Timeout, cts));

            if (completedTask != writeTask)
                throw new TimeoutException("Timeout while saving the file.");

            await writeTask;

            ApplyAttributesFile(configSaveFile);

            return ReturnMessageSuccess($@"SaveFileWriteAsync directory: {configSaveFile.PathFile}");
        }
        catch (UnauthorizedAccessException)
        {
            if(_count++ == 1) throw;
            var pathDefault = PreparationConfigSaveFile("");
            configSaveFile.SetPathFile(pathDefault);
            await SaveFileWriteAsync(configSaveFile, cts);

            return ReturnMessageSuccess(
                $@"UnauthorizedAccessException. SaveFileWriteAsync directory: {configSaveFile.PathFile}");
        }
        catch (DirectoryNotFoundException)
        {
            if(_count++ == 1) throw;
            var pathDefault = PreparationConfigSaveFile("");
            configSaveFile.SetPathFile(pathDefault);
            await SaveFileWriteAsync(configSaveFile, cts);

            return ReturnMessageSuccess(
                $@"DirectoryNotFoundException. SaveFileWriteAsync directory: {configSaveFile.PathFile}");
        }
        catch (Exception)
        {     
            throw new Exception();
        }
    }

    public async Task<string> SaveFileWriteBytesAsync(ConfigSaveFile configSaveFile, CancellationToken cts = default)
    {
        if (configSaveFile.DataBytes is null)
            throw new ArgumentNullException(nameof(configSaveFile), 
              "The property responsible for processing the bytes is null.");
        try
        {
            configSaveFile.SetPathFile(PreparationConfigSaveFile(configSaveFile.PathFile));
            var writeTask = File.WriteAllBytesAsync(configSaveFile.PathFile,
                configSaveFile.DataBytes!, cts);

            var completedTask = await Task.WhenAny(writeTask, Task.Delay(configSaveFile.Timeout, cts));

            if (completedTask != writeTask)
                throw new TimeoutException("Timeout while saving the file.");

            await writeTask;

            ApplyAttributesFile(configSaveFile);

            return ReturnMessageSuccess($@"SaveFileWriteByteAsync directory: {configSaveFile.PathFile}");
        }
        catch (UnauthorizedAccessException)
        {
            if(_count++ == 1) throw;
            var pathDefault = PreparationConfigSaveFile("");
            configSaveFile.SetPathFile(pathDefault);
            await SaveFileWriteBytesAsync(configSaveFile, cts);

            return ReturnMessageSuccess(
                $@"UnauthorizedAccessException. SaveFileWriteByteAsync directory: {configSaveFile.PathFile}");
        }
        catch (DirectoryNotFoundException)
        {
            if(_count++ == 1) throw;
            var pathDefault = PreparationConfigSaveFile("");
            configSaveFile.SetPathFile(pathDefault);
            await SaveFileWriteBytesAsync(configSaveFile);

            return ReturnMessageSuccess(
                $@"DirectoryNotFoundException. SaveFileWriteByteAsync directory: {configSaveFile.PathFile}");
        }
        catch (Exception)
        {
            throw new Exception();
        }
    }

    public string SaveFileWrite(ConfigSaveFile configSaveFile)
    {
        try
        {
            configSaveFile.SetPathFile(PreparationConfigSaveFile(configSaveFile.PathFile));
            File.WriteAllText(configSaveFile.PathFile, configSaveFile.Data);

            ApplyAttributesFile(configSaveFile);

            return ReturnMessageSuccess(
                $@"SaveFileWrite directory: {configSaveFile.PathFile}");
        }
        catch (UnauthorizedAccessException) when (_count == 0)
        {
            var pathDefault = PreparationConfigSaveFile("");
            configSaveFile.SetPathFile(pathDefault);
            SaveFileWrite(configSaveFile);

            return ReturnMessageSuccess(
                $@"UnauthorizedAccessException. SaveFileWrite directory: {configSaveFile.PathFile}");
        }
        catch (DirectoryNotFoundException) when (_count++ == 0)
        {
            var pathDefault = PreparationConfigSaveFile("");
            configSaveFile.SetPathFile(pathDefault);
            SaveFileWrite(configSaveFile);

            return ReturnMessageSuccess(
                $@"DirectoryNotFoundException. SaveFileWrite directory: {configSaveFile.PathFile}");
        }
        catch (Exception)
        {
            throw new Exception();
        }
    }

    public string SaveFileWriteBytes(ConfigSaveFile configSaveFile)
    {
        if (configSaveFile.DataBytes is null)
            throw new ArgumentNullException(nameof(configSaveFile.DataBytes),
              "The property responsible for processing the bytes is null.");
        try
        {
            configSaveFile.SetPathFile(PreparationConfigSaveFile(configSaveFile.PathFile));
            File.WriteAllBytes(configSaveFile.PathFile, configSaveFile.DataBytes!);

            ApplyAttributesFile(configSaveFile);

            return ReturnMessageSuccess($@"SaveFileWriteBytes directory: {configSaveFile.PathFile}");
        }
        catch (UnauthorizedAccessException) when (_count++ == 0)
        {
            var pathDefault = PreparationConfigSaveFile("");
            configSaveFile.SetPathFile(pathDefault);
            SaveFileWriteBytes(configSaveFile);

            return ReturnMessageSuccess(
                $@"UnauthorizedAccessException. SaveFileWriteBytes directory: {configSaveFile.PathFile}");
        }
        catch (DirectoryNotFoundException) when( _count++ == 0)
        {
            var pathDefault = PreparationConfigSaveFile("");
            configSaveFile.SetPathFile(pathDefault);
             SaveFileWriteBytes(configSaveFile);

            return ReturnMessageSuccess(
                $@"DirectoryNotFoundException. SaveFileWriteBytes directory: {configSaveFile.PathFile}");
        }
        catch (Exception)
        {
            throw new Exception();
        }
    }


    private static string PreparationConfigSaveFile(string? filePath)
    {
        var dir = string.IsNullOrWhiteSpace(filePath)
            ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources")
            : Path.GetDirectoryName(filePath)!;

        Directory.CreateDirectory(dir);
        return dir;
    }


    private static string ReturnMessageSuccess(string func) => $"File saved in the directory: {func}";

    private static void ApplyAttributesFile(ConfigSaveFile configSaveFile)
    {
        var combinedAttributes = configSaveFile.TypeFileAttributes.Aggregate((a, b) => a | b);
        File.SetAttributes(configSaveFile.PathFile, combinedAttributes);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var combinedUnixAttributes = configSaveFile.TypeFileUnixModes.Aggregate((a, b) => a | b);

            File.SetUnixFileMode(configSaveFile.PathFile, combinedUnixAttributes);
        }

        File.SetCreationTime(configSaveFile.PathFile, configSaveFile.Created);
    }
}
