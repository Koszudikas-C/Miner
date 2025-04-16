namespace LibTimeTask.Auth;

public static class AuthTimeClient
{
    public static Task AuthenticateClientTimeout => Task.Delay(TimeSpan.FromMinutes(5));

}