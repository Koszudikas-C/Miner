using DataFictitious.Entities.Remote.Client;
using LibHandler.Interface;
using LibRemoteAndClient.Entities.Client.Enum;
using LibRemoteAndClient.Entities.Remote.Client;
using Moq;
using Xunit;

namespace TestGlobalEventBus.Service;

public class TestGlobalEventBusClient
{
    private readonly Mock<IEventBus> _testIEventBusTes = new();
    
    [Fact]
    public void Subscribe_ShouldReceiveStringMessage_WhenStringIsPublished()
    {
        Action<string>? capturedHandler = null;
        
        _testIEventBusTes.Setup(m =>
            m.Subscribe(It.IsAny<Action<string>>()))
            .Callback<Action<string>>(handler => capturedHandler = handler);
        
        _testIEventBusTes.Object.Subscribe<string>(msg => { });
        
        const string expected = "Hello World!";
        string? received = null;
        
        capturedHandler = msg => received = msg;
        capturedHandler?.Invoke(expected);
        
        Assert.NotNull(received);
        Assert.Equal(expected, received);
        Assert.IsType<string>(received);
        Assert.StartsWith("Hello", received!);
        Assert.False(string.IsNullOrWhiteSpace(received));
        Assert.Matches(@"^Hello\s\w+!$", received);
        
        _testIEventBusTes.Verify(m => m.Subscribe(It.IsAny<Action<string>>()), Times.Once);
    }

    [Fact]
    public void Subscribe_ShouldReceiveClientCommandEnum_WhenEnumIsPublished()
    {
        Action<ClientCommand>? capturedHandler = null;
        
        _testIEventBusTes.Setup(m =>
            m.Subscribe(It.IsAny<Action<ClientCommand>>()))
            .Callback<Action<ClientCommand>>(handler => capturedHandler = handler);
        
        _testIEventBusTes.Object.Subscribe<Enum>(msg => { });
        
        const ClientCommand expected = ClientCommand.ClientMining;
        Enum? received = null;
        
        capturedHandler = msg => received = msg;
        capturedHandler?.Invoke(expected);
        
        Assert.NotNull(received);
        Assert.Equal(ClientCommand.ClientMining, received);
        Assert.IsType<ClientCommand>(received);
        
        _testIEventBusTes.Verify(m => m.Subscribe(It.IsAny<Action<Enum>>()), Times.Once);
    }
    
    [Fact]
    public void Subscribe_ShouldReceiveIntMessage_WhenIntIsPublished()
    {
        Action<int>? capturedHandler = null;
        
        _testIEventBusTes.Setup(m =>
            m.Subscribe(It.IsAny<Action<int>>()))
            .Callback<Action<int>>(handler => capturedHandler = handler);
        
        _testIEventBusTes.Object.Subscribe<int>(msg => { });
        
        const int expected = 1;
        int? received = null;
        
        capturedHandler = msg => received = msg;
        capturedHandler?.Invoke(expected);
        
        Assert.NotNull(received);
        Assert.Equal(expected, received);
        Assert.IsType<int>(received);
        
        _testIEventBusTes.Verify(m => m.Subscribe(It.IsAny<Action<int>>()), Times.Once);
    }
    
    [Fact]
    public void Subscribe_ShouldReceiveBoolMessage_WhenBoolIsPublished()
    {
        Action<bool>? capturedHandler = null;
        
        _testIEventBusTes.Setup(m =>
            m.Subscribe(It.IsAny<Action<bool>>()))
            .Callback<Action<bool>>(handler => capturedHandler = handler);
        
        _testIEventBusTes.Object.Subscribe<bool>(msg => { });
        
        const bool expected = true;
        bool? received = null;
        
        capturedHandler = msg => received = msg;
        capturedHandler?.Invoke(expected);
        
        Assert.NotNull(received);
        Assert.Equal(expected, received);
        Assert.True(received == expected);
        Assert.False(string.IsNullOrWhiteSpace(received?.ToString()));
        Assert.IsType<bool>(received);
        
        _testIEventBusTes.Verify(m => m.Subscribe(It.IsAny<Action<bool>>()), Times.Once);
    }

    [Fact]
    public void Subscribe_ShouldReceiveGuidMessage_WhenGuidIsPublished()
    {
        Action<Guid>? capturedHandler = null;

        _testIEventBusTes.Setup(m =>
                m.Subscribe(It.IsAny<Action<Guid>>()))
            .Callback<Action<Guid>>(handler => capturedHandler = handler);

        _testIEventBusTes.Object.Subscribe<Guid>(msg => { });

        var expected = Guid.NewGuid();
        Guid? received = null;

        capturedHandler = msg => received = msg;
        capturedHandler?.Invoke(expected);

        Assert.NotNull(received);
        Assert.Equal(expected, received);
        Assert.IsType<Guid>(received);
    }

    [Fact]
    public void Subscribe_ShouldReceiveObjectMessage_WhenObjectIsPublished()
    {
        Action<object>? capturedHandler = null;
        
        _testIEventBusTes.Setup(m =>
            m.Subscribe(It.IsAny<Action<object>>()))
            .Callback<Action<object>>(handler => capturedHandler = handler);
        
        _testIEventBusTes.Object.Subscribe<object>(msg => { });
        
        const string expected = "Hello World!";
        object? received = null;
        object obj = expected;
        
        capturedHandler = msg => received = msg;
        capturedHandler?.Invoke(expected);
        
        Assert.NotNull(received);
        Assert.Equal(expected, received);
        Assert.Same(obj, received);
        Assert.NotSame(new object(), received);
        Assert.IsType<string>(received);
        
        _testIEventBusTes.Verify(m => m.Subscribe(It.IsAny<Action<object>>()), Times.Once);
    }

    [Fact]
    public void Subscribe_ShouldReceiveClientInfo_WhenClientInfoIsPublished()
    {
        Action<ClientInfo>? captureHandler;
        
        _testIEventBusTes.Setup(m =>
            m.Subscribe(It.IsAny<Action<ClientInfo>>()))
            .Callback<Action<ClientInfo>>(handler => captureHandler = handler);
        
        _testIEventBusTes.Object.Subscribe<ClientInfo>(msg => { });

        var expected = ClientInfoTest.ClientInfo();
        ClientInfo? received = null;
        
        captureHandler = msg => received = msg;
        captureHandler?.Invoke(expected);
        
        Assert.NotNull(received);
        Assert.Equal(expected.Id, received!.Id);
        Assert.NotEqual(Guid.Empty, received.Id);
        Assert.Equal(expected.ClientMine, received.ClientMine);
        Assert.IsType<ClientInfo>(received);
        
        _testIEventBusTes.Verify(m => m.Subscribe(It.IsAny<Action<ClientInfo>>()), Times.Once);
    }
}