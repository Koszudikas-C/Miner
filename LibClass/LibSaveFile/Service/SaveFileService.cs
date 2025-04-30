using System.Runtime.InteropServices;
using LibManagerFile.Entities;
using LibManagerFile.Interface;

namespace LibSaveFile.Service;

public class SaveFileService : ISaveFile
{
    public async Task<bool> SaveFileWriteAsync(ConfigSaveFile configSaveFile, CancellationToken cts = default)
    {
        configSaveFile = PreparationConfigSaveFile(configSaveFile);

        var writeTask = File.WriteAllTextAsync(configSaveFile.PathFile!, configSaveFile.Data, cts);

        var completedTask = await Task.WhenAny(writeTask, Task.Delay(configSaveFile.Timeout, cts));

        if (completedTask != writeTask)
            throw new TimeoutException("Timeout while saving the file.");

        await writeTask;

        ApplyAttributesFile(configSaveFile);

        return true;
    }

    public async Task<bool> SaveFileWriteByteAsync(ConfigSaveFile configSaveFile, CancellationToken cts = default)
    {
        if (configSaveFile.DataBytes is null)
            throw new ArgumentNullException(nameof(configSaveFile.DataBytes));


        configSaveFile = PreparationConfigSaveFile(configSaveFile);

        var writeTask = File.WriteAllBytesAsync(configSaveFile.PathFile!, configSaveFile.DataBytes!, cts);

        var completedTask = await Task.WhenAny(writeTask, Task.Delay(configSaveFile.Timeout, cts));

        if (completedTask != writeTask)
            throw new TimeoutException("Timeout while saving the file.");

        await writeTask;

        ApplyAttributesFile(configSaveFile);

        return true;
    }

    public bool SaveFileWrite(ConfigSaveFile configSaveFile)
    {
        configSaveFile = PreparationConfigSaveFile(configSaveFile);

        File.WriteAllText(configSaveFile.PathFile!, configSaveFile.Data);

        ApplyAttributesFile(configSaveFile);

        return true;
    }

    public bool SaveFileByteWrite(ConfigSaveFile configSaveFile)
    {
        if (configSaveFile.DataBytes is null)
            throw new ArgumentNullException(nameof(configSaveFile.DataBytes));

        try
        {
            configSaveFile = PreparationConfigSaveFile(configSaveFile);

            File.WriteAllBytes(configSaveFile.PathFile!, configSaveFile.DataBytes!);

            ApplyAttributesFile(configSaveFile);

            return true;
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to save the file: {e.Message}");
        }
    }


    private static ConfigSaveFile PreparationConfigSaveFile(ConfigSaveFile configSaveFile)
    {
        if(string.IsNullOrWhiteSpace(configSaveFile.PathFile))
            configSaveFile.SetPathFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "Resources"));

        Directory.CreateDirectory(Path.GetDirectoryName(configSaveFile.PathFile)!);

        if (File.Exists(configSaveFile.PathFile))
            File.Delete(configSaveFile.PathFile);
        
        return configSaveFile;
    }

    private static void ApplyAttributesFile(ConfigSaveFile configSaveFile)
    {
        var combinedAttributes = configSaveFile.TypeFileAttributes.Aggregate((a, b) => a | b);
        File.SetAttributes(configSaveFile.PathFile!, combinedAttributes);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var combinedUnixAttributes = configSaveFile.TypeFileUnixModes.Aggregate((a, b) => a | b);

            File.SetUnixFileMode(configSaveFile.PathFile!, combinedUnixAttributes);
        }

        File.SetCreationTime(configSaveFile.PathFile!, configSaveFile.Created);
    }
}