namespace LibSocket.Connection;

public static class TargetHost
{
    public static readonly string Host = Environment.GetEnvironmentVariable("DOMAIN_HOST")!;
}