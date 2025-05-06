using LibClassProcessOperations.Interface;

namespace ApiRemoteWorkClientBlockChain.Service
{
    public interface IOperationFactory
    {
        Task<IProcessOptions> CreateAuthSocks5Operation();
        Task<IProcessOptions> CreateCheckAppClientBlockChainOperation();
        Task<IProcessOptions> CreateDownloadAppClientBlockChainOperation();
        Task<IProcessOptions> CreateLogsOperation();
        Task<IProcessOptions> CreateStatusConnectionOperation();
        Task<IProcessOptions> CreateStatusTransactionOperation();
    }
}
