using LibClassProcessOperations.Interface;

namespace ApiRemoteWorkClientBlockChain.Factory.Abstract;

public abstract class ProcessFactory
{
    public abstract IProcessOptions CreateProcess();
}