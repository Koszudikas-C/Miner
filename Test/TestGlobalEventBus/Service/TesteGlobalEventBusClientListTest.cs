using DataFictitious.Entities.Remote.Client;
using LibHandler.Interface;
using LibRemoteAndClient.Entities.Remote.Client;
using Moq;
using Xunit;

namespace TestGlobalEventBus.Service;

public class TesteGlobalEventBusClientListTest
{
    private readonly Mock<IEventBus> _testIEventBusTes = new();

    [Fact]
    public void SubscribeList_ShouldReceiveStrings_WhenEventIsTriggered()
    {
        Action<List<string>>? capturedHandler = null;

        _testIEventBusTes.Setup(m =>
                m.SubscribeList(It.IsAny<Action<List<string>>>()))
            .Callback<Action<List<string>>>(handler => capturedHandler = handler);

        _testIEventBusTes.Object.SubscribeList<string>(msg => { });

        var expected = new List<string> { "one", "two", "three" };
        List<string>? received = null;

        capturedHandler = msg => received = msg;
        capturedHandler?.Invoke(expected);

        Assert.NotNull(received);
        Assert.Equal(expected.Count, received!.Count);
        Assert.Contains("two", received);
        Assert.StartsWith("o", received.First());
        Assert.All(received, item => Assert.False(string.IsNullOrWhiteSpace(item)));
        Assert.IsType<List<string>>(received);

        _testIEventBusTes.Verify(m => m.SubscribeList(It.IsAny<Action<List<string>>>()), Times.Once);
    }

    [Fact]
    public void SubscribeList_ShouldReceiveIntegers_WhenEventIsTriggered()
    {
        Action<List<int>>? capturedHandler = null;

        _testIEventBusTes.Setup(m =>
                m.SubscribeList(It.IsAny<Action<List<int>>>()))
            .Callback<Action<List<int>>>(handler => capturedHandler = handler);

        _testIEventBusTes.Object.SubscribeList<int>(msg => { });

        var expected = new List<int> { 1, 2, 3 };
        List<int>? received = null;

        capturedHandler = msg => received = msg;
        capturedHandler?.Invoke(expected);

        Assert.NotNull(received);
        Assert.Equal(expected.Count, received!.Count);
        Assert.Contains(2, received);
        Assert.DoesNotContain(5, received);
        Assert.All(received, num => Assert.True(num > 0));
        Assert.IsType<List<int>>(received);

        _testIEventBusTes.Verify(m => m.SubscribeList(It.IsAny<Action<List<int>>>()), Times.Once);
    }

    [Fact]
    public void SubscribeList_ShouldReceiveObjects_WhenEventIsTriggered()
    {
        Action<List<object>>? capturedHandler = null;

        _testIEventBusTes.Setup(m =>
                m.SubscribeList(It.IsAny<Action<List<object>>>()))
            .Callback<Action<List<object>>>(handler => capturedHandler = handler);

        _testIEventBusTes.Object.SubscribeList<object>(msg => { });

        var expected = new List<object> { "text", 123, true };
        List<object>? received = null;

        capturedHandler = msg => received = msg;
        capturedHandler?.Invoke(expected);

        Assert.NotNull(received);
        Assert.Equal(3, received!.Count);
        Assert.Contains("text", received);
        Assert.Contains(123, received);
        Assert.Contains(true, received);
        Assert.Collection(received,
            item => Assert.IsType<string>(item),
            item => Assert.IsType<int>(item),
            item => Assert.IsType<bool>(item)
        );

        _testIEventBusTes.Verify(m => m.SubscribeList(It.IsAny<Action<List<object>>>()), Times.Once);
    }

    [Fact]
    public void SubscribeList_ShouldReceiveClientInfoList_WhenEventIsTriggered()
    {
        Action<List<ClientInfo>>? capturedHandler = null;

        _testIEventBusTes.Setup(m =>
                m.SubscribeList(It.IsAny<Action<List<ClientInfo>>>()))
            .Callback<Action<List<ClientInfo>>>(handler => capturedHandler = handler);

        _testIEventBusTes.Object.SubscribeList<ClientInfo>(msg => { });

        var expected = new List<ClientInfo>
        {
            new ClientInfo { Id = Guid.NewGuid() },
            new ClientInfo { Id = Guid.NewGuid() }
        };

        List<ClientInfo>? received = null;

        capturedHandler = msg => received = msg;
        capturedHandler?.Invoke(expected);

        Assert.NotNull(received);
        Assert.Equal(expected.Count, received!.Count);
        Assert.All(received, client =>
        {
            Assert.NotEqual(Guid.Empty, client.Id);
        });
        Assert.IsType<List<ClientInfo>>(received);

        _testIEventBusTes.Verify(m => m.SubscribeList(It.IsAny<Action<List<ClientInfo>>>()), Times.Once);
    }

    [Fact]
    public void SubscribeList_ShouldReceiveGuids_WhenEventIsTriggered()
    {
        Action<List<Guid>>? capturedHandler = null;

        _testIEventBusTes.Setup(m =>
                m.SubscribeList(It.IsAny<Action<List<Guid>>>()))
            .Callback<Action<List<Guid>>>(handler => capturedHandler = handler);

        _testIEventBusTes.Object.SubscribeList<Guid>(msg => { });

        var expected = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        List<Guid>? received = null;

        capturedHandler = msg => received = msg;
        capturedHandler?.Invoke(expected);

        Assert.NotNull(received);
        Assert.Equal(3, received!.Count);
        Assert.All(received, id => Assert.NotEqual(Guid.Empty, id));
        Assert.IsType<List<Guid>>(received);

        _testIEventBusTes.Verify(m => m.SubscribeList(It.IsAny<Action<List<Guid>>>()), Times.Once);
    }

    [Fact]
    public void SubscribeList_ShouldReceiveClientInfoListCorrectly_WhenUsingStaticClientInfoList()
    {
        Action<List<ClientInfo>>? capturedHandler = null;

        _testIEventBusTes.Setup(m =>
                m.SubscribeList(It.IsAny<Action<List<ClientInfo>>>()))
            .Callback<Action<List<ClientInfo>>>(handler => capturedHandler = handler);

        _testIEventBusTes.Object.SubscribeList<ClientInfo>(msg => { });

        var expected = ClientInfoTest.GetClientInfoList();
        List<ClientInfo>? received = null;

        capturedHandler = msg => received = msg;
        capturedHandler?.Invoke(expected);

        Assert.NotNull(received);
        Assert.Equal(expected.Count, received!.Count);
        Assert.All(received, client =>
        {
            Assert.NotEqual(Guid.Empty, client.Id);
        });

        _ = expected.Zip(received, (exp, rec) =>
        {
            Assert.Equal(exp.Id, rec.Id);
            Assert.Equal(exp.SocketWrapper!.InnerSocket, rec.SocketWrapper!.InnerSocket);
            Assert.Equal(exp.SslStreamWrapper, rec.SslStreamWrapper);
            Assert.Equal(exp.ClientMine, rec.ClientMine);
            return true;
        });

        Assert.IsType<List<ClientInfo>>(received);

        _testIEventBusTes.Verify(m => m.SubscribeList(
            It.IsAny<Action<List<ClientInfo>>>()), Times.Once);
    }
}