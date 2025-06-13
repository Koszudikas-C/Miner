namespace ApiRemoteWorkClientBlockChain.Entities;

public class Client(string ip, string port)
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Ip { get; set; } = ip;

    public string Port { get; set; } = port;

    public string? TimeoutReceive { get; set; }

    public string? TimeoutSend { get; set; }

    public DateTime DateConnected { get; set; } = DateTime.UtcNow;

}