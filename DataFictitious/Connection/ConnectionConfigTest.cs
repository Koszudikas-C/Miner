using LibSocketAndSslStream.Entities;

namespace DataFictitious.Connection;

public static class ConnectionConfigTest
{
    public static ConnectionConfig GetConnectionTest() 
        => new ConnectionConfig(){ Port = 5051, MaxConnections = 5};
    
    public static List<ConnectionConfig> GetConnectionListTest() 
        => [new ConnectionConfig { Port = 4599, MaxConnections = 50 },
            new ConnectionConfig { Port = 4594, MaxConnections = 1000 }];
}