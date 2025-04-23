namespace LibSocketAndSslStream.Entities;

public class ConnectionConfig
{
    public uint Port { get; init; }
    public int MaxConnections { get; init; }
}