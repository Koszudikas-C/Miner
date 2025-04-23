using System.Net.Sockets;

namespace LibSocket.Interface;

public interface ISocketWrapper
{
    bool Connected { get; }

    Socket InnerSocket { get; }
    // Adicione outros métodos/propriedades necessários
}