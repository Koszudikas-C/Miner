namespace LibClassManagerOptions.Entities.Enum;

public enum TypeManagerResponseOperations
{
    Success = 0,
    NotFound = 1,
    InvalidRequest = 2,
    Error = 3,
    Unauthorized = 4,
    Timeout = 5,
    AlreadyExists = 6,
    ValidationFailed = 7,
    PartialSuccess = 8,
    Pending = 9,
    SslStreamNotAuthenticated = 10,
    SocketNotConnected = 11,
    PortNotOpen = 12,
}