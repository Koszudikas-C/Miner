namespace LibTimeTask.Auth;

public class AuthTimeRemote
{
    public static Task AuthenticateRemoteTimeout => Task.Delay(TimeSpan.FromMinutes(1));
}