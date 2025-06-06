namespace LibDtoRemote.Dto;

public class ConfigCryptographDto
{
    public List<ConfigCryptographDto> X1 = [];
    public byte[]? HeaderSignature { get; set; }
    public string? Key { get; set; }
    public string? HmacKey { get; set; }
    public byte[]? EncryptKey { get; set; }
    public string? FilePath { get; set; }
    public object? Data { get; set; }
    public byte[]? DataBytes { get; set; }
}
