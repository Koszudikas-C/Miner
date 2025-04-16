using System.Runtime.InteropServices;
using LibRemoteAndClient.Entities.Client.Abstract;

namespace LibRemoteAndClient.Entities.Client.Library;

public partial class LibraryExternWin : LibraryExtern
{
    private const string LibraryName = "kernel32.dll";
    private const int Flags = 0x00000008;

    [LibraryImport(LibraryName, EntryPoint = "LoadLibraryExW",
        SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    private static partial IntPtr LoadLibraryExWindows(string lpFileName,
        IntPtr hFile, uint dwFlags);

    [LibraryImport(LibraryName, EntryPoint = "FreeLibrary",
        SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool FreeLibraryWindows(IntPtr hModule);

    [LibraryImport(LibraryName, EntryPoint = "GetProcAddress",
        SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    private static partial IntPtr GetProcAddressWindows(IntPtr hModule,
        string procName);

    public override IntPtr Load(string path)
    {
        var handle = LoadLibraryExWindows(path, IntPtr.Zero, Flags);

        return handle;
    }

    public override bool Free(IntPtr handle)
    {
        if (handle == IntPtr.Zero) return false;

        var result = FreeLibraryWindows(handle);

        if (result) return true;

        var error = Marshal.GetLastWin32Error();
        throw new ExternalException($"FreeLibrary failed with error code: {error}");

    }

    public override IntPtr GetLibraryAddress(IntPtr handle,
        string dllName) => GetProcAddressWindows(handle, dllName);

    public override string GetError() => Marshal.GetLastWin32Error().ToString();

}