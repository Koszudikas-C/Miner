namespace LibRemoteAndClient.Entities.Client;

public record ClientHandshakeRequest
{
    public string? HashExecHex { get; set; }

    public string? Nonce { get; set; }

    public string? SignatureHex { get; set; }
}