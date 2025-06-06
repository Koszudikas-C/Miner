using System.Runtime.InteropServices;
using LibDirectoryFileRemote.Interface;

namespace ApiRemoteWorkClientBlockChain.Service;

public class DirectoryFileService(ILogger<DirectoryFileService> logger) : IDirectoryFile
{
    private readonly string _pathDefault =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Installer");

    public async Task<string> GetDirectoryDefaultTorrAsync()
    {
        try
        {
            var result = Path.Combine(_pathDefault, GetFileNameDefaultTorr());

            CreateDirectory();
            FileExist(result);
            
            await Task.CompletedTask;

            return result;
        }
        catch (Exception e)
        {
            throw new Exception($"There was an error when seeking the TOR program board. Error:{e.Message}");
        }
    }

    public string GetFileNameDefaultTorr() =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "tor-expert-bundle-windows.zip": "tor-expert-bundle-linux.zip";


    public string GetDirectoryDefaultTorr()
    {
        try
        {
            var result = Path.Combine(_pathDefault, GetFileNameDefaultTorr());

            CreateDirectory();
            FileExist(result);

            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


    private void CreateDirectory()
    {
        try
        {
            Directory.CreateDirectory(_pathDefault);
        }
        catch (Exception e)
        {
            throw new Exception($"There was an error related to the creation of the base directory. Error:{e.Message}");
        }
    }

    private static void FileExist(string fileName)
    {
        try
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException(nameof(_pathDefault));
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception($"Listen to one in the {fileName} file validation. Error:{e.Message}");
        }
    }
}
