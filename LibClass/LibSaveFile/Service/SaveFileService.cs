using System.Runtime.InteropServices;
using LibManagerFile.Entities;
using LibManagerFile.Interface;

namespace LibSaveFile.Service;

public class SaveFileService : ISaveFile
{
    public async Task<string> SaveFileWriteAsync(ConfigSaveFile configSaveFile,
        CancellationToken cts = default)
    {
        try
        {
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
            var pathDefault = PreparationConfigSaveFile("");
            configSaveFile.SetPathFile(pathDefault);
            await SaveFileWriteAsync(configSaveFile, cts);

            return ReturnMessageSuccess(
                $@"UnauthorizedAccessException. SaveFileWriteAsync directory: {configSaveFile.PathFile}");
        }
        catch (DirectoryNotFoundException)
        {
            var pathDefault = PreparationConfigSaveFile("");
            configSaveFile.SetPathFile(pathDefault);
            await SaveFileWriteAsync(configSaveFile, cts);

            return ReturnMessageSuccess(
                $@"DirectoryNotFoundException. SaveFileWriteAsync directory: {configSaveFile.PathFile}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<string> SaveFileWriteBytesAsync(ConfigSaveFile configSaveFile, CancellationToken cts = default)
    {
        try
        {
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
            var pathDefault = PreparationConfigSaveFile("");
            configSaveFile.SetPathFile(pathDefault);
            await SaveFileWriteBytesAsync(configSaveFile, cts);

            return ReturnMessageSuccess(
                $@"UnauthorizedAccessException. SaveFileWriteByteAsync directory: {configSaveFile.PathFile}");
        }
        catch (DirectoryNotFoundException)
        {
            var pathDefault = PreparationConfigSaveFile("");
            configSaveFile.SetPathFile(pathDefault);
            await SaveFileWriteBytesAsync(configSaveFile);

            return ReturnMessageSuccess(
                $@"DirectoryNotFoundException. SaveFileWriteByteAsync directory: {configSaveFile.PathFile}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public string SaveFileWrite(ConfigSaveFile configSaveFile)
    {
        try
        {
            File.WriteAllText(configSaveFile.PathFile, configSaveFile.Data);

            ApplyAttributesFile(configSaveFile);

            return ReturnMessageSuccess(
                $@"SaveFileWrite directory: {configSaveFile.PathFile}");
        }
        catch (UnauthorizedAccessException)
        {
            var pathDefault = PreparationConfigSaveFile("");
            configSaveFile.SetPathFile(pathDefault);
            SaveFileWrite(configSaveFile);

            return ReturnMessageSuccess(
                $@"UnauthorizedAccessException. SaveFileWrite directory: {configSaveFile.PathFile}");
        }
        catch (DirectoryNotFoundException)
        {
            var pathDefault = PreparationConfigSaveFile("");
            configSaveFile.SetPathFile(pathDefault);
            SaveFileWrite(configSaveFile);

            return ReturnMessageSuccess(
                $@"DirectoryNotFoundException. SaveFileWrite directory: {configSaveFile.PathFile}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public string SaveFileWriteBytes(ConfigSaveFile configSaveFile)
    {
        try
        {
            File.WriteAllBytes(configSaveFile.PathFile, configSaveFile.DataBytes!);

            ApplyAttributesFile(configSaveFile);

            return ReturnMessageSuccess($@"SaveFileWriteBytes directory: {configSaveFile.PathFile}");
        }
        catch (UnauthorizedAccessException)
        {
            var pathDefault = PreparationConfigSaveFile("");
            configSaveFile.SetPathFile(pathDefault);
            SaveFileWriteBytes(configSaveFile);

            return ReturnMessageSuccess(
                $@"UnauthorizedAccessException. SaveFileWriteBytes directory: {configSaveFile.PathFile}");
        }
        catch (DirectoryNotFoundException)
        {
            var pathDefault = PreparationConfigSaveFile("");
            configSaveFile.SetPathFile(pathDefault);
             SaveFileWriteBytes(configSaveFile);

            return ReturnMessageSuccess(
                $@"DirectoryNotFoundException. SaveFileWriteBytes directory: {configSaveFile.PathFile}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


    private static string PreparationConfigSaveFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "Resources");

        Directory.CreateDirectory(filePath);

        if (File.Exists(filePath))
            File.Delete(filePath);

        return filePath;
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

    private static string DirectoryDefault(string fileName) =>
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", fileName);
}