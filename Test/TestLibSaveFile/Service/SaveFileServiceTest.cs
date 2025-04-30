using DataFictitious.LibClass.LibSaveFile;
using LibManagerFile.Entities.Enum;
using LibSaveFile.Service;
using Xunit;

namespace TestLibSaveFile.Service;

public class SaveFileServiceTest
{
    private readonly SaveFileService _saveFile = new ();
    
    [Fact]
    public async Task save_file_write_async()
    {
        var configSaveFile = ConfigSaveFileTest.GetConfigSaveFileTest();
        
        var result = await _saveFile.SaveFileWriteAsync(configSaveFile);
        
        Assert.True(result);
    }

    [Fact]
    public async Task save_file_write_async_different_directory()
    {
        var configSaveFile = ConfigSaveFileTest.GetConfigSaveFileTest();
        configSaveFile.SetPathFile("tmp");

        var result = await _saveFile.SaveFileWriteAsync(configSaveFile);
        
        Assert.True(result);
    }

    [Fact]
    public async Task save_file_write_async_different_extension()
    {
        var configSaveFile = ConfigSaveFileTest.GetConfigSaveFileTest();
        configSaveFile.SetExtension(TypeExtensionFile.Txt);

        var result = await _saveFile.SaveFileWriteAsync(configSaveFile);
        
        Assert.True(result);
    }

    [Fact]
    public void save_file_write()
    {
        var configSaveFile = ConfigSaveFileTest.GetConfigSaveFileTest();

        var result = _saveFile.SaveFileWrite(configSaveFile);
        
        Assert.True(result);
    }

    [Fact]
    public void save_file_different_directory()
    {
        var configSaveFile = ConfigSaveFileTest.GetConfigSaveFileTest();
        configSaveFile.SetPathFile("/var/tmp");

        var result = _saveFile.SaveFileWrite(configSaveFile);
        
        Assert.True(result);
    }

    [Fact]
    public void save_file_different_extension()
    {
        var configSaveFile = ConfigSaveFileTest.GetConfigSaveFileTest();
        
        configSaveFile.SetExtension(TypeExtensionFile.Pdf);

        var result = _saveFile.SaveFileWrite(configSaveFile);
        
        Assert.True(result);
    }

    [Fact]
    public async Task save_file_bytes_async()
    {
        var configSaveFile = ConfigSaveFileTest.GetConfigSaveFileTest();
        
        var result = await _saveFile.SaveFileWriteByteAsync(configSaveFile);
        
        Assert.True(result);
    }

    [Fact]
    public async Task save_file_write_byte_async_should_throw_if_bytes_null()
    {
        var configSaveFile = ConfigSaveFileTest.GetConfigSaveFileTest();
        configSaveFile.DataBytes = null;
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _saveFile.SaveFileWriteByteAsync(configSaveFile));
    }

    [Fact]
    public void save_file_byte_write_should_throw_if_bytes_null()
    {
        var configSaveFile = ConfigSaveFileTest.GetConfigSaveFileTest();
        configSaveFile.DataBytes = null;
        Assert.Throws<ArgumentNullException>(() => _saveFile.SaveFileByteWrite(configSaveFile));
    }

    [Fact]
    public void save_file_byte_write_should_throw_on_exception()
    {
        var configSaveFile = ConfigSaveFileTest.GetConfigSaveFileTest();
        configSaveFile.DataBytes = [ 1, 2, 3 ];
        configSaveFile.SetPathFile("/invalid_path/invalid_file.bin");
        Assert.Throws<Exception>(() => _saveFile.SaveFileByteWrite(configSaveFile));
    }
}