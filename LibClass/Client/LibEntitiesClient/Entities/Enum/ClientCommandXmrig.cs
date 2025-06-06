namespace LibEntitiesClient.Entities.Enum;

public enum ClientCommandXmrig
{
    Stop = -1,
    Start = 0,
    Running = 1,
    NotRunning = -2,
    NoValidConfig = 3,
    Erro = 4,
    ErroLoadLib = 5
}
