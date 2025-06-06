namespace LibRemoteAndClient.Entities.Remote.Client;

public class GuidTokenAuth
{
    public int Id { get; set; }
    public Guid GuidTokenGlobal { get; set; } = GuidToken.GuidTokenGlobal;
}
