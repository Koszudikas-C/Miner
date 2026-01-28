using System.Data;
using System.Runtime.InteropServices;
using LibManagerFile.Entities.Enum;

namespace LibManagerFile.Entities;

public class ConfigSaveFile
{
    public string FileName { get; set; }
    public TypeExtensionFile ExtensionFile { get; private set; }
    public string? Data { get; set; }
    public byte[]? DataBytes { get; set; }
    public string PathFile { get; private set; }
    public DateTime Created { get; set; } = DateTime.Now;
    public List<FileAttributes> TypeFileAttributes { get; set; } = [FileAttributes.None];
    public List<UnixFileMode> TypeFileUnixModes { get; set; } = [UnixFileMode.None];
    public int Timeout { get; set; } = 5000;

    public ConfigSaveFile(string fileName, string pathFile)
    {
        FileName = fileName;
        PathFile = pathFile;
        SetPathFile(pathFile);
    }

public void SetPathFile(string directory)
{
    if (string.IsNullOrWhiteSpace(directory))
        throw new InvalidExpressionException(
            $"The directory cannot be null, empty or whitespace: '{directory}'");
    
    var cleanDirectory = directory.TrimStart('/', '\\');
    
    PathFile = Path.Combine(cleanDirectory, $"{FileName}.{ExtensionFile}");
}


    public void SetExtension(TypeExtensionFile typeExtensionFile)
    {
        if (!System.Enum.IsDefined(typeof(TypeExtensionFile), typeExtensionFile))
            throw new InvalidExpressionException(
                $"Check the past typeExtensionFile, it cannot be null or empty:{typeExtensionFile}"
            );

        if (Path.HasExtension(FileName))
            FileName = Path.GetFileNameWithoutExtension(FileName);

        ExtensionFile = typeExtensionFile;
        FileName = $"{FileName}.{ExtensionFile}";
    }
}