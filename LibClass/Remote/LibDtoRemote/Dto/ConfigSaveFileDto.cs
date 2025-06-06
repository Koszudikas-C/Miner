using LibDtoRemote.Dto.Enum;

namespace LibDtoRemote.Dto;

public class ConfigSaveFileDto
{
    public string? FileName { get; set; }
    public TypeExtensionFileDto ExtensionFile { get; set; }
    public string? Data { get; set; }
    public byte[]? DataBytes { get; set; } 
    public string? PathFile { get; set; }
    public DateTime Created { get; set; }
    
    public List<FileAttributes> TypeFileAttributes { get; set; } = [FileAttributes.None];
    public List<UnixFileMode> TypeFileUnixModes { get; set; } = [UnixFileMode.None];
    public int Timeout { get; set; }
}
