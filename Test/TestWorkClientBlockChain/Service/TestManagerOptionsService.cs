using LibClassManagerOptions.Entities.Enum;
using LibClassProcessOperations.Interface;
using LibCryptography.Interface;
using LibDto.Dto;
using LibManagerFile.Interface;
using LibSend.Interface;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using WorkClientBlockChain.Service;
using WorkClientBlockChain.Connection.Interface;
using WorkClientBlockChain.Utils.Interface;
using LibDto.Dto.Enum;
using LibMapperObj.Interface;

namespace TestWorkClientBlockChain.Service;

public class ManagerOptionsServiceTests<T>
{
    private readonly Mock<ILogger<ManagerOptionsService<T>>> _mockLogger = new();
    private readonly Mock<ISend<ParamsManagerOptionsResponseDto>> _mockSend = new(); 
    private readonly Mock<IClientConnected> _mockClientContext = new();
    private readonly Mock<ICryptographFile> _mockCryptographFile = new();
    private readonly Mock<ISearchFile> _mockSearchFile = new();
    private readonly Mock<IProcessOptionsClient> _mockProcessOptionsClient = new();
    private readonly Mock<IMapperObj> _mockMapperObj = new();
    private readonly ManagerOptionsService<T> _managerOptionsService;

    public ManagerOptionsServiceTests()
    {
        _managerOptionsService = new ManagerOptionsService<T>(
            _mockLogger.Object,
            _mockSend.Object,
            _mockClientContext.Object,
            _mockCryptographFile.Object,
            _mockSearchFile.Object,
            _mockProcessOptionsClient.Object,
            _mockMapperObj.Object
            );
    }
    
    public void InitializeOptions_ValidType_CallsExpectedMethod()
    {
        
       // _ = _managerOptionsService.InitializeOptionsAsync();
       //
       //  switch (option)
       //  {
       //      case TypeManagerOptions.AuthSocks5:
       //          // _mockProcessOptions.Verify(x =>
       //          //     x.IsProcessAuthSocks5Async(It.IsAny<CancellationToken>()), Times.Once);
       //          break;
       //      case TypeManagerOptions.CheckAppClientBlockChain:
       //
       //          break;
       //      case TypeManagerOptions.DownloadAppClientBlockChain:
       //          break;
       //      case TypeManagerOptions.Logs:
       //
       //          break;
       //      case TypeManagerOptions.StatusConnection:
       //
       //          break;
       //      case TypeManagerOptions.StatusTransaction:
       //
       //          break;
       //  }
    }

    // [Fact]
    // public void InitializeOptions_CancelOperations_ResetsToken()
    // {
    //     // _managerOptionsService.InitializeOptionsAsync();
    //     //
    //     // _mockProcessOptions.VerifyNoOtherCalls(); 
    // }

    // [Fact]
    // public void InitializeOptions_InvalidType_ThrowsArgumentException_AndLogsError()
    // {
    //     // const TypeManagerOptions invalidOption = (TypeManagerOptions)999;
    //     //
    //     // var exception = Assert.Throws<ArgumentException>(() =>
    //     //     _managerOptionsService.InitializeOptions(invalidOption));
    //     //
    //     // Assert.Contains("invalid", exception.Message.ToLower());
    //     //
    //     // _mockLogger.Verify(
    //     //     x => x.Log(
    //     //         LogLevel.Error,
    //     //         It.IsAny<EventId>(),
    //     //         It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("invalid")),
    //     //         null,
    //     //         It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
    //     //     Times.Once);
    // }
}
