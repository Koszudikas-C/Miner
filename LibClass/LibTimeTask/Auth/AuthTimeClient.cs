namespace LibTimeTask.Auth;

public static class AuthTimeClient
{
    public static Task AuthenticateClientTimeout => Task.Delay(TimeSpan.FromSeconds(30));

}