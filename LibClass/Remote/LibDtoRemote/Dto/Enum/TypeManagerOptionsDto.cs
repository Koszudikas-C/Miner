namespace LibDtoRemote.Dto.Enum;

public enum TypeManagerOptionsDto
{
    Error = 0x0F,
    CancelOperations = 0x0E,
    
    AuthSocks5 = 0,
    CheckAppClientBlockChain = 1,
    DownloadAppClientBlockChain = 2,
    Logs = 3,
    StatusConnection = 4,
    StatusTransaction = 5,
}
