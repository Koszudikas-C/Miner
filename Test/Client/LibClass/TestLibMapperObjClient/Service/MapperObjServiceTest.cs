using DataFictitiousClient.LibClass.LibCryptograph;
using LibCryptographyClient.Entities;
using LibDtoClient.Dto;
using LibManagerFileClient.Interface;
using LibMapperObjClient.Interface;
using LibMapperObjClient.Service;
using LibSearchFileClient.Service;
using Xunit;

namespace TestLibMapperObjClient.Service;

public class MapperObjServiceTest
{
    private readonly ISearchFile _searchFile = new SearchFileService();
    private readonly IMapperObj _mapperObj = new MapperObjService();
    private readonly ConfigCryptographTest _configCryptographTest;

    private static string ApBaseDirectory => Path.Combine(
        Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "..", "..", "Resources", "koewa.json");

    private static readonly string DirectoryApp = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "Resources", "koewa.json");

    public MapperObjServiceTest()
    {
        _configCryptographTest = new ConfigCryptographTest(
            _searchFile,
            _mapperObj
        );
    }

    private static void Restore_test_file<T>()
    {
        try
        {
            var configCryptograph = new ConfigCryptograph(DirectoryApp);

            if (File.Exists(configCryptograph.FilePath))
                File.Delete(configCryptograph.FilePath);

            File.Copy(ApBaseDirectory, configCryptograph.FilePath);
        }
        catch (DirectoryNotFoundException e)
        {
            var dir = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "Resources");

            Directory.CreateDirectory(dir);
            throw new DirectoryNotFoundException();
        }
    }
    
    [Fact]
    public void mapper_obj_map_to_dto()
    {
        Restore_test_file<ConfigVariableDto>();

        var source = _configCryptographTest.GetConfigCryptographConfigVariable();
        var dto = new ConfigCryptographDto();
        source.GetEncryptKey();

        var result = _mapperObj.MapToDto(source, dto);

        Assert.NotNull(result.Data);
        Assert.Equal(source.FilePath, result.FilePath);
        Assert.Equal(source.GetDataBytes(), result.DataBytes);
        Assert.Equal(source.HeaderSignature, result.HeaderSignature);
        Assert.Equal(source.HeaderSignature.Length, result.HeaderSignature!.Length);
        Assert.Equal(source.Key, result.Key);
        Assert.Equal(source.HmacKey, result.HmacKey);
        Assert.Equal(source.GetEncryptKey(), result.EncryptKey);
    }

    [Fact]
    public void map_to_dto_must_map_corresponding_properties()
    {
        var mapper = new MapperObjService();
        var origen = new ConfigCryptograph("teste.conf")
        {
            Key = "SecretKey123",
            HmacKey = "HMAC456"
        };


        var dto = mapper.MapToDto(origen, new ConfigCryptographDto());

        Assert.Equal(origen.Key, dto.Key);
        Assert.Equal(origen.HmacKey, dto.HmacKey);
        Assert.Equal(origen.FilePath, dto.FilePath);
    }

    [Fact]
    public void map_to_obj_dev_mappeary_properties()
    {
        var mapper = new MapperObjService();
        var dto = new ConfigCryptographDto
        {
            Key = "NewKey",
            HmacKey = "NewHMAC",
            FilePath = "new_path.conf"
        };

        var config = mapper.MapToObj(
            dto,
            new ConfigCryptograph("original_path.conf")
        );

        Assert.Equal(dto.Key, config.Key);
        Assert.Equal(dto.HmacKey, config.HmacKey);
        Assert.Equal("new_path.conf", config.FilePath);
    }

    [Fact]
    public void mapToDto_with_exclusion_of_properties_should_ignore_specified_properties()
    {
        var mapper = new MapperObjService();
        var origem = new ConfigCryptograph("teste.conf")
        {
            Key = "SecretKey",
            HmacKey = "HMAC123"
        };

        var exclusoes = new List<string> { nameof(ConfigCryptograph.HmacKey) };

        var dto = mapper.MapToDto(origem, new ConfigCryptographDto(), exclusoes);

        Assert.Equal(origem.Key, dto.Key);
        Assert.Null(dto.HmacKey);
    }

    [Fact]
    public void map_to_obj_from_origem_null_exception()
    {
        var mapper = new MapperObjService();
        ConfigCryptographDto dto = null!;

        Assert.Throws<ArgumentNullException>(() =>
            mapper.MapToObj(dto, new ConfigCryptograph("test.conf"))
        );
    }

    [Fact]
    public void MapToConfigCryptograph_DeveMapearCorretamenteEAplicarValidacoes()
    {
        var mapper = new MapperObjService();
        var dto = new ConfigCryptographDto
        {
            FilePath = "caminho_valido.conf",
            Key = "Chave32Caracteres12345678901234",
            HmacKey = "Hmac64Caracteres123456789012345678901234567890123456789012345678"
        };

        var config = mapper.MapToObj(dto, new ConfigCryptograph("teste.conf"));

        Assert.Equal(dto.FilePath, config.FilePath);
        Assert.Equal(dto.Key, config.Key);
        Assert.Equal(dto.HmacKey, config.HmacKey);

        Assert.NotNull(config.GetEncryptKey());
        Assert.NotNull(config.GetHmacKey());
    }
}