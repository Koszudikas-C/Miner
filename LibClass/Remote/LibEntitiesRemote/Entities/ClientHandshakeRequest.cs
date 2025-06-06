namespace LibEntitiesRemote.Entities;

public record ClientHandshakeRequest(string HashExecHex, string SignatureHex)
{
    public string HashExecHex { get; set; } = HashExecHex;

    public Guid Nonce { get; set; }

    public string SignatureHex { get; set; } = SignatureHex;
}
