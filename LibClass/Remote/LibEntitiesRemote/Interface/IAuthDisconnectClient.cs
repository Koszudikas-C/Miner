namespace LibEntitiesRemote.Interface;

public interface IAuthDisconnectClient
{
  void Disconnect();
  bool Connected { get; }
  Guid Id { get; }
}
