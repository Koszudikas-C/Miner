using System.Text;
using LibManagerFileClient.Entities;

namespace DataFictitiousClient.LibClass.LibSaveFile;

public static class ConfigSaveFileTest
{
    public static ConfigSaveFile GetConfigSaveFileTest()
    {
        var json = """
                   {
                       "Name": "Example",
                       "Enabled": true,
                       "MaxConnections": 10,
                       "Servers": [
                           "server1.example.com",
                           "server2.example.com"
                       ],
                       "Settings": {
                           "Timeout": 30,
                           "Retry": 5
                       }
                   }
                   """;

        var configSaveFile = new ConfigSaveFile("teste", @"tmp/null")
        {
            Data = json,
            DataBytes = Encoding.UTF8.GetBytes(json)
        };

        return configSaveFile;
    }

    public static string ReturnMessageSuccess(string func) => $"File saved in the directory: {func}";
    
    public static string SaveFileWriteAsyncSuccess(ConfigSaveFile configSaveFile) =>
        ReturnMessageSuccess($@"SaveFileWriteAsync directory: {configSaveFile.PathFile}");

    public static string SaveFileWriteAsyncSuccessUnauthorized(ConfigSaveFile configSaveFile) =>
        ReturnMessageSuccess($@"UnauthorizedAccessException. SaveFileWriteAsync directory: {configSaveFile.PathFile}");

    public static string SaveFileWriteAsyncSuccessDirectoryNotFound(ConfigSaveFile configSaveFile) =>
        ReturnMessageSuccess($@"DirectoryNotFoundException. SaveFileWriteAsync directory: {configSaveFile.PathFile}");

    public static string SaveFileWriteBytesAsyncSuccess(ConfigSaveFile configSaveFile) =>
        ReturnMessageSuccess($@"SaveFileWriteByteAsync directory: {configSaveFile.PathFile}");

    public static string SaveFileWriteByteAsyncSuccessUnauthorized(ConfigSaveFile configSaveFile) =>
        ReturnMessageSuccess(
            $@"UnauthorizedAccessException. SaveFileWriteByteAsync directory: {configSaveFile.PathFile}");

    public static string SaveFileWriteByteAsyncSuccessDirectoryNotFound(ConfigSaveFile configSaveFile) =>
        ReturnMessageSuccess(
            $@"DirectoryNotFoundException. SaveFileWriteByteAsync directory: {configSaveFile.PathFile}");

    public static string SaveFileWriteSuccess(ConfigSaveFile configSaveFile) =>
        ReturnMessageSuccess($@"SaveFileWrite directory: {configSaveFile.PathFile}");

    public static string SaveFileWriteSuccessUnauthorized(ConfigSaveFile configSaveFile) =>
        ReturnMessageSuccess($@"UnauthorizedAccessException. SaveFileWrite directory: {configSaveFile.PathFile}");

    public static string SaveFileWriteSuccessDirectoryNotFound(ConfigSaveFile configSaveFile) =>
        ReturnMessageSuccess($@"DirectoryNotFoundException. SaveFileWrite directory: {configSaveFile.PathFile}");

    public static string SaveFileWriteBytesSuccess(ConfigSaveFile configSaveFile) =>
        ReturnMessageSuccess($@"SaveFileWriteBytes directory: {configSaveFile.PathFile}");

    public static string SaveFileWriteBytesSuccessUnauthorized(ConfigSaveFile configSaveFile) =>
        ReturnMessageSuccess($@"UnauthorizedAccessException. SaveFileWriteBytes directory: {configSaveFile.PathFile}");

    public static string SaveFileWriteBytesSuccessDirectoryNotFound(ConfigSaveFile configSaveFile) =>
        ReturnMessageSuccess($@"DirectoryNotFoundException. SaveFileWriteBytes directory: {configSaveFile.PathFile}");
}
