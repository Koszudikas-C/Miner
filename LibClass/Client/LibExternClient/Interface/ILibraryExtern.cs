namespace LibExternClient.Interface;

public interface ILibraryExtern
{
  IntPtr Load(string path);
  bool Free(IntPtr handle);
  IntPtr GetLibraryAddress(IntPtr handle, string dllName);

  string GetError();
}
