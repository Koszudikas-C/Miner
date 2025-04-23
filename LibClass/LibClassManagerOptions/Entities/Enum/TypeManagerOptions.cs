namespace WorkClientBlockChain.Entities.Enum;

public enum TypeManagerOptions
{
    Error = 0x0F,
    CancelOperations = 0x0E,
    
    Auth = 0,
    CheckAppClientBlockChain = 1,
    DownloadAppClientBlockChain = 2,
    Logs = 3,
    StatusConnection = 4,
    StatusTransaction = 5,
}