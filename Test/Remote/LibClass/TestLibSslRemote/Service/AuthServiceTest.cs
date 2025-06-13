using System.Net.Security;
using System.Net.Sockets;
using DataFictitiousRemote.LibClass.LibCertificate;
using DataFictitiousRemote.LibClass.LibSocket;
using DataFictitiousRemote.LibClass.LibSsl;
using LibCertificateRemote.Interface;
using LibEntitiesRemote.Interface;
using LibSocketAndSslStreamRemote.Interface;
using LibSslRemote.Service;
using Moq;
using Xunit;

namespace TestLibSslRemote.Service;

public class AuthServiceTest
{
    private readonly Mock<ICertificate> _certificateMock = new();
    private readonly Mock<ISocketWrapper> _socketWrapperMock = new();
    private readonly Mock<ISslServerAuthOptions> _sslRemoteAuthMock = new();
    private readonly IAuth _authService;
    private readonly ISslServerAuthOptions _sslRemoteAuthOptions;

    public AuthServiceTest()
    {
        _authService = new AuthService(
            _sslRemoteAuthMock.Object
            );
        
        _sslRemoteAuthOptions = new SslServerAuthOptionsService(
            _certificateMock.Object
            );
    }
    
    [Fact]
    public async Task authenticate_socket_wrapper_async_should_throw_if_socket_wrapper_null()
    {
        var ctsSource = new CancellationTokenSource();
        
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _authService.AuthenticateAsync(_socketWrapperMock.Object, ctsSource.Token));
    }

    [Fact]
    public async Task authenticate_socket_wrapper_async_should_throw_if_socket_connected_false()
    {
        var ctsSource = new CancellationTokenSource();
        using var innerSocket = GetSocketReal();
        
        _socketWrapperMock.Setup(x => x.InnerSocket).Returns(innerSocket);
        _sslRemoteAuthMock.Setup(x => 
            x.GetConfigSslServerAuthenticationOptions()).Returns(new SslServerAuthenticationOptions());
        
        await Assert.ThrowsAsync<SocketException>(() =>
            _authService.AuthenticateAsync(_socketWrapperMock.Object, ctsSource.Token));
    }

    [Fact]
    public async Task authenticate_socket_wrapper_async_should_throw_if_timeout_operation()
    {
        var ctsSource = new CancellationTokenSource();
        var socketClient = await SocketTest.GetSocketClientConnected(ctsSource.Token);
        var certificate = CertificateTest.LoadCertificate();
        
        _socketWrapperMock.Setup(x => x.InnerSocket).Returns(socketClient);
        _sslRemoteAuthMock.Setup(x => x.GetConfigSslServerAuthenticationOptions())
            .Returns(SslStreamTest.GetConfigSslServerAuthenticationOptions(certificate));
        
        _certificateMock
            .Setup(c => c.LoadCertificate())
            .Returns(certificate);
        
        await ctsSource.CancelAsync();
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _authService.AuthenticateAsync(_socketWrapperMock.Object, ctsSource.Token));
        
        SocketTest.DisposableSockets();
    }
    
    
    private static Socket GetSocketReal() => new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
}