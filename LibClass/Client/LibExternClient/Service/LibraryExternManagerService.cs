using System.Runtime.InteropServices;
using LibExternClient.Interface;

namespace LibExternClient.Service;

public  class LibraryExternManagerService(ILibraryExtern libraryExtern) : ILibraryExternManager
{
    private  ILibraryExtern? _libraryExternWin = libraryExtern;

    private  ILibraryExtern? _libraryExternLinux = libraryExtern;

    private  IntPtr _handle = IntPtr.Zero;

    public  IntPtr Load()
    {
        _handle = IntPtr.Zero;
        var fileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "libxmrig.dll"
            : "libxmrig.so";

        var libraryPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "libs", fileName);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _libraryExternWin = new LibraryExternWinService();
            _handle = _libraryExternWin.Load(libraryPath);

            CheckLibrary($"Error loading library: {_libraryExternWin.GetError()}");

            return _handle;
        }

        _libraryExternLinux = new LibraryExternLinuxService();
        _handle = _libraryExternLinux.Load(libraryPath);

        CheckLibrary($"Error loading library: {_libraryExternLinux.GetError()}");

        return _handle;
    }

    public void CheckLibrary(string message)
    {
        if (_handle != IntPtr.Zero) return;

        throw new Exception(message);
    }

    public  bool Free(IntPtr handle)
    {
        if (handle == IntPtr.Zero) return false;

        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? _libraryExternWin!.Free(handle)
            : _libraryExternLinux!.Free(handle);
    }

    public  IntPtr GetLibraryAddress(string funcdllName)
    {
        CheckLibrary("Error not loading library.");
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? _libraryExternWin!.GetLibraryAddress(_handle, funcdllName)
            : _libraryExternLinux!.GetLibraryAddress(_handle, funcdllName);
    }
}
