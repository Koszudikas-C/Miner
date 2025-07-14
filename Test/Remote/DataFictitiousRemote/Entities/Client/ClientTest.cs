using LibEntitiesRemote.Entities.Client.Enum;

namespace DataFictitiousRemote.Entities.Client;

public static class ClientTest
{
   public static List<LibEntitiesRemote.Entities.Client.Client> GetClients()
    {
        return
        [
            new LibEntitiesRemote.Entities.Client.Client("192.168.0.1", "10.0.0.1", "8080")
            {
                Id = 1,
                AttemptsConnection = 2,
                StateClient = ConnectionStates.Connected,
                TimeoutReceive = 3000,
                TimeoutSend = 3000,
                PrimaryConnection = DateTimeOffset.UtcNow.AddHours(-2),
                DateConnected = DateTimeOffset.UtcNow.AddHours(-1)
            },

            new LibEntitiesRemote.Entities.Client.Client("172.16.10.5", "10.0.0.2", "443")
            {
                Id = 2,
                AttemptsConnection = 5,
                StateClient = ConnectionStates.Disconnected,
                TimeoutReceive = 4000,
                TimeoutSend = 4000,
                PrimaryConnection = DateTimeOffset.UtcNow.AddDays(-1),
                DateConnected = DateTimeOffset.UtcNow.AddHours(-23)
            },

            new LibEntitiesRemote.Entities.Client.Client("203.0.113.99", "10.0.0.3", "22")
            {
                Id = 3,
                AttemptsConnection = 1,
                StateClient = ConnectionStates.Faulted,
                TimeoutReceive = null,
                TimeoutSend = null,
                PrimaryConnection = DateTimeOffset.UtcNow.AddMinutes(-50),
                DateConnected = DateTimeOffset.UtcNow.AddMinutes(-45)
            },

            new LibEntitiesRemote.Entities.Client.Client("198.51.100.77", "10.0.0.4", "3306")
            {
                Id = 4,
                AttemptsConnection = 7,
                StateClient = ConnectionStates.Faulted,
                TimeoutReceive = 6000,
                TimeoutSend = 6000,
                PrimaryConnection = DateTimeOffset.UtcNow.AddHours(-4),
                DateConnected = DateTimeOffset.UtcNow.AddHours(-3)
            },

            new LibEntitiesRemote.Entities.Client.Client("8.8.8.8", "10.0.0.5", "80")
            {
                Id = 5,
                AttemptsConnection = 0,
                StateClient = ConnectionStates.Connected,
                TimeoutReceive = 2500,
                TimeoutSend = 2500,
                PrimaryConnection = DateTimeOffset.UtcNow,
                DateConnected = DateTimeOffset.UtcNow
            }
        ];
    }

    public static LibEntitiesRemote.Entities.Client.Client GetConnectedClient()
    {
        return new LibEntitiesRemote.Entities.Client.Client("192.0.2.1", "10.0.0.10", "8081")
        {
            Id = 6,
            AttemptsConnection = 1,
            StateClient = ConnectionStates.Connected,
            TimeoutReceive = 2000,
            TimeoutSend = 2000
        };
    }

    public static LibEntitiesRemote.Entities.Client.Client GetDisconnectedClient()
    {
        return new LibEntitiesRemote.Entities.Client.Client("203.0.113.10", "10.0.0.11", "443")
        {
            Id = 7,
            AttemptsConnection = 4,
            StateClient = ConnectionStates.Disconnected,
            TimeoutReceive = 3000,
            TimeoutSend = 3000
        };
    }

    public static LibEntitiesRemote.Entities.Client.Client GetFailedClient()
    {
        return new LibEntitiesRemote.Entities.Client.Client("198.51.100.200", "10.0.0.12", "3306")
        {
            Id = 8,
            AttemptsConnection = 10,
            StateClient = ConnectionStates.Faulted,
            TimeoutReceive = 6000,
            TimeoutSend = 6000
        };
    }
}