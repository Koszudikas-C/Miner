using System.Net;
using LibCryptography.Interface;
using LibDto.Dto.ClientMine;
using LibManagerFile.Interface;
using LibMapperObj.Interface;
using LibMapperObj.Service;
using LibReceive.Interface;
using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Entities.Client;
using LibSend.Interface;
using Microsoft.Extensions.Logging;
using Moq;
using WorkClientBlockChain.Service;
using WorkClientBlockChain.Utils;
using WorkClientBlockChain.Utils.Interface;
using Xunit;

namespace TestWorkClientBlockChain.Utils;

public class TestPosAuth
{
    private readonly IPosAuth _posAuth;
    private readonly IMapperObj _mapperObj = new MapperObjService();
    private readonly Mock<IReceive> _mockReceive = new();
    private readonly Mock<ISend<HttpStatusCode>> _mocSendHttpStatusCode = new();
    private readonly Mock<ILogger<PosAuthService>> _mockIloggerPostAuth = new();
    private readonly Mock<ISaveFile> _mockSaveFile = new();
    private readonly Mock<ICryptographFile> _mockCryptographFile = new();
    private readonly Mock<IMapperObj> _mockMapperObj = new();
    private readonly Mock<ISend<ClientMineDto>> _mockSendClientMineDto = new();


    public TestPosAuth()
    {
        _posAuth = new PosAuthService(
            _mockReceive.Object,
            _mocSendHttpStatusCode.Object,
            _mockIloggerPostAuth.Object,
            _mockSaveFile.Object,
            _mockCryptographFile.Object,
            _mapperObj,
            _mockSendClientMineDto.Object
        );
    }

    [Fact]
    public void send_clientMine_ok()
    {
        var clientInfo = new ClientInfo();
        var clientMine = new LibRemoteAndClient.Entities.Client.ClientMine();
        
        // _mockMapperObj.Setup(s
        //     => s.MapToDto(clientMine, new ClientMineDto()))
        //     .Returns<LibRemoteAndClient.Entities.Client.ClientMine, ClientMineDto>(
        //     (soruce, target) =>
        //     {
        //         target.Id = soruce.Id;
        //         target.Name = soruce.Name;
        //         target.HardwareInfoDto!.TotalDiskSpace = soruce.HardwareInfo!.TotalDiskSpace;
        //         target.HardwareInfoDto!.CpuInfo!.Name = soruce.HardwareInfo!.CpuInfo!.Name;
        //         return target;
        //     });
        _posAuth.SendClientMine(clientInfo);
    }
}