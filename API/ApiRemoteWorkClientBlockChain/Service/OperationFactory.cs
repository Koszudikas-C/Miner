using ApiRemoteWorkClientBlockChain.Interface;
using LibClassProcessOperations.Interface;

namespace ApiRemoteWorkClientBlockChain.Service
{
    public class OperationFactory : IOperationFactory
    {
        public async Task<IProcessOptions> CreateAuthSocks5Operation()
        {
            return await Task.FromResult<IProcessOptions>(null); 
        }

        public async Task<IProcessOptions> CreateCheckAppClientBlockChainOperation()
        {
            return await Task.FromResult<IProcessOptions>(null);
        }

        public async Task<IProcessOptions> CreateDownloadAppClientBlockChainOperation()
        {
            return await Task.FromResult<IProcessOptions>(null);
        }

        public async Task<IProcessOptions> CreateLogsOperation()
        {
            return await Task.FromResult<IProcessOptions>(null);
        }

        public async Task<IProcessOptions> CreateStatusConnectionOperation()
        {
            return await Task.FromResult<IProcessOptions>(null); 
        }

        public async Task<IProcessOptions> CreateStatusTransactionOperation()
        {
            return await Task.FromResult<IProcessOptions>(null);
        }
    }
}
