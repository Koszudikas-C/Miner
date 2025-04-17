using Microsoft.AspNetCore.Http;

namespace LibMiddleware.Interface;

public interface IApiKey
{
    Task InvokeAsync(HttpContext context);
    bool IsValid(string? apiKey);
}