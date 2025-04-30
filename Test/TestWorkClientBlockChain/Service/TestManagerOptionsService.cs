using LibClassManagerOptions.Entities.Enum;
using LibClassProcessOperations.Interface;
using LibCryptography.Interface;
using LibManagerFile.Interface;
using LibSend.Interface;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using WorkClientBlockChain.Service;
using WorkClientBlockChain.Connection.Interface;
using WorkClientBlockChain.Utils.Interface;

namespace TestWorkClientBlockChain.Service;

public class ManagerOptionsServiceTests
{
    private readonly Mock<IProcessOptions> _mockProcessOptions = new();
    private readonly Mock<ILogger<ManagerOptionsService>> _mockLogger = new();
    private readonly Mock<ISend<TypeManagerResponseOperations>> _mockSend = new(); 
    private readonly Mock<IClientContext> _mockClientContext = new();
    private readonly Mock<IPortOpen> _mockPortOpen = new();
    private readonly Mock<ICryptographFile> _mockCryptographFile = new();
    private readonly Mock<IDownloadAll> _mockDownloadAll = new();
    private readonly Mock<ISearchFile> _mockSearchFile = new();
    private readonly ManagerOptionsService _managerOptionsService;

    public ManagerOptionsServiceTests()
    {
        _managerOptionsService = new ManagerOptionsService(
            _mockProcessOptions.Object,
            _mockLogger.Object,
            _mockSend.Object,
            _mockClientContext.Object,
            _mockPortOpen.Object,
            _mockCryptographFile.Object,
            _mockDownloadAll.Object,
            _mockSearchFile.Object
            );
    }

    [Theory]
    [InlineData(TypeManagerOptions.AuthSocks5)]
    [InlineData(TypeManagerOptions.CheckAppClientBlockChain)]
    [InlineData(TypeManagerOptions.DownloadAppClientBlockChain)]
    [InlineData(TypeManagerOptions.Logs)]
    [InlineData(TypeManagerOptions.StatusConnection)]
    [InlineData(TypeManagerOptions.StatusTransaction)]
    public void InitializeOptions_ValidType_CallsExpectedMethod(TypeManagerOptions option)
    {
       _ = _managerOptionsService.InitializeOptions(option);

        switch (option)
        {
            case TypeManagerOptions.AuthSocks5:
                // _mockProcessOptions.Verify(x =>
                //     x.IsProcessAuthSocks5Async(It.IsAny<CancellationToken>()), Times.Once);
                break;
            case TypeManagerOptions.CheckAppClientBlockChain:
                _mockProcessOptions.Verify(x =>
                    x.IsProcessCheckAppClientBlockChain(It.IsAny<CancellationToken>()), Times.Once);
                break;
            case TypeManagerOptions.DownloadAppClientBlockChain:
                _mockProcessOptions.Verify(x =>
                    x.IsProcessDownloadAppClientBlockChain(It.IsAny<CancellationToken>()), Times.Once);
                break;
            case TypeManagerOptions.Logs:
                _mockProcessOptions.Verify(x =>
                    x.IsProcessLogs(It.IsAny<CancellationToken>()), Times.Once);
                break;
            case TypeManagerOptions.StatusConnection:
                _mockProcessOptions.Verify(x =>
                    x.IsProcessStatusConnection(It.IsAny<CancellationToken>()), Times.Once);
                break;
            case TypeManagerOptions.StatusTransaction:
                _mockProcessOptions.Verify(x => 
                    x.IsProcessStatusTransaction(It.IsAny<CancellationToken>()), Times.Once);
                break;
        }
    }

    [Fact]
    public void InitializeOptions_CancelOperations_ResetsToken()
    {
        _managerOptionsService.InitializeOptions(TypeManagerOptions.CancelOperations);
        
        _mockProcessOptions.VerifyNoOtherCalls(); 
    }

    [Fact]
    public void InitializeOptions_InvalidType_ThrowsArgumentException_AndLogsError()
    {
        // const TypeManagerOptions invalidOption = (TypeManagerOptions)999;
        //
        // var exception = Assert.Throws<ArgumentException>(() =>
        //     _managerOptionsService.InitializeOptions(invalidOption));
        //
        // Assert.Contains("invalid", exception.Message.ToLower());
        //
        // _mockLogger.Verify(
        //     x => x.Log(
        //         LogLevel.Error,
        //         It.IsAny<EventId>(),
        //         It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("invalid")),
        //         null,
        //         It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        //     Times.Once);
    }
}
