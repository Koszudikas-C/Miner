namespace ApiRemoteWorkClientBlockChain.Entities;

public class ClientNotAuthorized(string ipRemote, string message)
{
    public string IpRemote { get; set; } = ipRemote;
    public string Message { get; set; } = message;
}