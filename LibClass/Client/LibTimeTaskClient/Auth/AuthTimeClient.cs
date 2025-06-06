namespace LibTimeTaskClient.Auth;

public static class AuthTime
{
    public static Task AuthenticateClientTimeout(CancellationToken cts = default)
      => Task.Delay(TimeSpan.FromSeconds(10), cts);

}
