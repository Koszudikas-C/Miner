namespace LibDto.Dto;

public class ClientHandshakeDto
{
    public string? HashExecHex { get; set; }

    public Guid? Nonce { get; set; }

    public string? SignatureHex { get; set; }
}