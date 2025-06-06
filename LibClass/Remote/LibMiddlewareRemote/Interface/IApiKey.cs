using Microsoft.AspNetCore.Http;

namespace LibMiddlewareRemote.Interface;

public interface IApiKey
{
    Task InvokeAsync(HttpContext context);
    bool IsValid(string? apiKey);
}
