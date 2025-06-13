using DataFictitiousRemote.LibClass.LibSaveFile;
using LibManagerFileRemote.Entities.Enum;
using LibManagerFileRemote.Interface;
using LibSaveFileRemote.Service;
using Xunit;

namespace TestLibSaveFile.Service;

public class SaveFileServiceTest
{
  private readonly ISaveFile _saveFile = new SaveFileService();

  [Fact]
  public async Task save_file_write_async()
  {
    var configSaveFile = ConfigSaveFileTest.GetConfigSaveFileTest();

    var result = await _saveFile.SaveFileWriteAsync(configSaveFile);

    Assert.NotNull(result);
    Assert.Equal(ConfigSaveFileTest.SaveFileWriteAsyncSuccess(configSaveFile), result);
  }

  [Fact]
  public async Task save_file_write_async_different_directory()
  {
    var configSaveFile = ConfigSaveFileTest.GetConfigSaveFileTest();
    configSaveFile.SetPathFile("tmp");

    var result = await _saveFile.SaveFileWriteAsync(configSaveFile);

    Assert.NotNull(result);
    Assert.Equal(ConfigSaveFileTest.SaveFileWriteAsyncSuccess(configSaveFile), result);
  }

  [Fact]
  public async Task save_file_write_async_different_extension()
  {
    var configSaveFile = ConfigSaveFileTest.GetConfigSaveFileTest();
    configSaveFile.SetExtension(TypeExtensionFile.Txt);

    var result = await _saveFile.SaveFileWriteAsync(configSaveFile);

    Assert.NotNull(result);

    Assert.Equal(ConfigSaveFileTest.SaveFileWriteAsyncSuccess(configSaveFile), result);
  }

  [Fact]
  public void save_file_write()
  {
    var configSaveFile = ConfigSaveFileTest.GetConfigSaveFileTest();

    var result = _saveFile.SaveFileWrite(configSaveFile);

    Assert.NotNull(result);
    Assert.Equal(ConfigSaveFileTest.SaveFileWriteSuccess(configSaveFile), result);
  }

  [Fact]
  public void save_file_different_directory()
  {
    var configSaveFile = ConfigSaveFileTest.GetConfigSaveFileTest();

    configSaveFile.SetPathFile(Path.GetTempPath());

    var result = _saveFile.SaveFileWrite(configSaveFile);

    Assert.NotNull(result);
    Assert.Equal(ConfigSaveFileTest.SaveFileWriteSuccess(configSaveFile), result);
  }

  [Fact]
  public void save_file_different_extension()
  {
    var configSaveFile = ConfigSaveFileTest.GetConfigSaveFileTest();

    configSaveFile.SetExtension(TypeExtensionFile.Pdf);

    var result = _saveFile.SaveFileWrite(configSaveFile);

    Assert.NotNull(result);
    
    Assert.Equal(ConfigSaveFileTest.SaveFileWriteSuccess(configSaveFile), result);
  }


  [Fact]
  public async Task save_file_write_byte_async_should_throw_if_bytes_null()
  {
    var configSaveFile = ConfigSaveFileTest.GetConfigSaveFileTest();
    configSaveFile.DataBytes = null;
    await Assert.ThrowsAsync<ArgumentNullException>(() => _saveFile.SaveFileWriteBytesAsync(configSaveFile));
  }

  [Fact]
  public void save_file_byte_write_should_throw_if_bytes_null()
  {
    var configSaveFile = ConfigSaveFileTest.GetConfigSaveFileTest();
    configSaveFile.DataBytes = null;
    Assert.Throws<ArgumentNullException>(() => _saveFile.SaveFileWriteBytes(configSaveFile));
  }

  [Fact]
  public void SaveFileWrite_Should_Throw_When_Data_IsNull()
  {
    var configSaveFile = ConfigSaveFileTest.GetConfigSaveFileTest();
    configSaveFile.Data = null;
    var result = _saveFile.SaveFileWrite(configSaveFile);

    Assert.NotNull(result);
    Assert.Equal(ConfigSaveFileTest.SaveFileWriteSuccess(configSaveFile), result);
  }

  [Fact]
  public async Task SaveFileWriteAsync_Should_Throw_When_Data_IsNull()
  {
    var configSaveFile = ConfigSaveFileTest.GetConfigSaveFileTest();
    configSaveFile.Data = null;
    var result = await _saveFile.SaveFileWriteAsync(configSaveFile);

    Assert.NotNull(result);
    Assert.Equal(ConfigSaveFileTest.SaveFileWriteAsyncSuccess(configSaveFile), result);
  }
}
