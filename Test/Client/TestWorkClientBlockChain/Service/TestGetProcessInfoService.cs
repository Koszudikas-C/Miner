using WorkClientBlockChain.Interface;
using WorkClientBlockChain.Service;
using Xunit;

namespace TestWorkClientBlockChain.Service;

public class TestGetProcessInfoService
{
    private readonly IGetProcessInfo _processInfoPort = new GetProcessInfoService();
    
    [Fact]
    public void check_process_name()
    {
        var result = _processInfoPort.GetProcessInfo("dotnet");

        Assert.NotNull(result);
        Assert.Equal("dotnet", result.FirstOrDefault()!.Name);
    }
    
    [Fact]
    public void check_process_port()
    {
        var result = _processInfoPort.GetProcessInfo(9050);
        
        Assert.NotNull(result);
    }
}
