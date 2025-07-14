using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace LibMiddlewareRemote.MiddleWare;

public class ApiKeyMiddleware(
  RequestDelegate next,
  IConfiguration configuration)
{
  private readonly string _apiKey = configuration["security:ApiKey"]
                                    ?? Environment.GetEnvironmentVariable("ApiKey")
                                    ?? throw new Exception("API Key not configured");

  public async Task InvokeAsync(HttpContext context)
  {
    if (!context.Request.Headers.TryGetValue("Authorization", out var extractedApiKey)
        || extractedApiKey != _apiKey)
    {
      context.Response.StatusCode = 401;
      await context.Response.WriteAsync("Unauthorized");
      return;
    }

    await next(context);
  }
}
