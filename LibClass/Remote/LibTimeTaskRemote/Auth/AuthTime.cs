namespace LibTimeTaskRemote.Auth;

public class AuthTime
{
    public static Task AuthenticateRemoteTimeout (CancellationToken cts = default)
      => Task.Delay(TimeSpan.FromSeconds(30), cts);
}
