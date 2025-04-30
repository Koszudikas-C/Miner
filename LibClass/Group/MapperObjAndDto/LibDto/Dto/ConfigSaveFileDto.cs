using LibManagerFile.Entities.Enum;

namespace LibDto.Dto;

public class ConfigSaveFileDto
{
    public string? FileName { get; set; }
    public TypeExtensionFile ExtensionFile { get; set; }
    public string? Data { get; set; }
    public byte[]? DataBytes { get; set; } 
    public string? PathFile { get; private set; }
    public DateTime Created { get; set; } = DateTime.Now;
    
    public List<FileAttributes> TypeFileAttributes { get; set; } = [FileAttributes.None];
    public List<UnixFileMode> TypeFileUnixModes { get; set; } = [UnixFileMode.None];
    public int Timeout { get; set; } = 5000;
}