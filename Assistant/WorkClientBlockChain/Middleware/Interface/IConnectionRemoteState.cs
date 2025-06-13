using System.Data;
using LibCommunicationStateClient.Entities.Enum;

namespace WorkClientBlockChain.Middleware.Interface;

public interface IConnectionRemoteState
{
    Task MonitoringConnectionWorkAsync(ConnectionStates state, CancellationToken cts = default);
}
