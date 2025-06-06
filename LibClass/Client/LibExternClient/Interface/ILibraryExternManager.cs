namespace LibExternClient.Interface;

public interface ILibraryExternManager
{
  IntPtr Load();

  void CheckLibrary(string message);

  bool Free(IntPtr handle);

  IntPtr GetLibraryAddress(string funcdllName);
}
