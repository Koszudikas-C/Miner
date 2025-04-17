using System.Net;

namespace LibCommunicationStatus.Entities;

public class ApiResponse<T>(
    HttpStatusCode statusCode,
    bool success,
    string message,
    IEnumerable<T>? data = null,
    List<string>? errors = null)
{
    public HttpStatusCode StatusCode { get; private set; } = statusCode;
    public bool Success { get; private set; } = success;
    public string Message { get; set; } = message;
    public IEnumerable<T>? Data { get; set; } = data;
    public List<string>? Errors { get; set; } = errors;


    public static ApiResponse<T> CreateSuccessResponse(HttpStatusCode statusCode, IEnumerable<T>? data = null,
        string message = "Operation successful")
    {
        return new ApiResponse<T>(statusCode, true, message, data);
    }

    public static ApiResponse<T> CreateErrorResponse(HttpStatusCode statusCode, List<string> errors,
        string message = "Operation failed")
    {
        return new ApiResponse<T>(statusCode, false, message, null, errors);
    }
}