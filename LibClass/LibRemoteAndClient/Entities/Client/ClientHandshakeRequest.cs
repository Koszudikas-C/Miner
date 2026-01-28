namespace LibRemoteAndClient.Entities.Client;

public record ClientHandshake
{
    public string? HashExecHex { get; set; }

    public string? Nonce { get; set; }

    public string? SignatureHex { get; set; }
}