using DataFictitious.LibClass.LibCryptograph;
using LibCryptography.Entities;
using LibDto.Dto;
using LibManagerFile.Interface;
using LibMapperObj.Interface;
using LibMapperObj.Service;
using LibSearchFile.Service;
using LibSocketAndSslStream.Entities;
using Xunit;

namespace TestLibMapperObj.Service;

public class MapperObjServiceTest
{
    private readonly ISearchFile _searchFile = new SearchFileService();
    private readonly IMapperObj _mapperObj = new MapperObjService();
    private readonly ConfigCryptographTest _configCryptographTest;

    private static string ApBaseDirectory => Path.Combine(
        Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "Resources", "koewa.json");

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
        var configCryptograph = new ConfigCryptograph(DirectoryApp);

        if (File.Exists(configCryptograph.FilePath))
            File.Delete(configCryptograph.FilePath);

        File.Copy(ApBaseDirectory, configCryptograph.FilePath);
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
    public void mapToDto_must_map_corresponding_properties()
    {
        var mapper = new MapperObjService();
        var origem = new ConfigCryptograph("teste.conf")
        {
            Key = "SecretKey123",
            HmacKey = "HMAC456"
        };

        
        var dto = mapper.MapToDto(origem, new ConfigCryptographDto());
        
        Assert.Equal(origem.Key, dto.Key);
        Assert.Equal(origem.HmacKey, dto.HmacKey);
        Assert.Equal(origem.FilePath, dto.FilePath);
    }

    [Fact]
    public void mapToObj_deve_MapearPropriedadesCorrespondentes()
    {
        var mapper = new MapperObjService();
        var dto = new ConfigCryptographDto
        {
            Key = "NovaChave",
            HmacKey = "NovoHMAC",
            FilePath = "novo_caminho.conf"
        };
        
        var config = mapper.MapToObj(
            dto, 
            new ConfigCryptograph("caminho_original.conf")
        );
        
        Assert.Equal(dto.Key, config.Key);
        Assert.Equal(dto.HmacKey, config.HmacKey);
        Assert.Equal("novo_caminho.conf", config.FilePath); // FilePath deve ser atualizado
    }
    
    [Fact]
    public void mapToDto_with_exclusion_of_properties_should_ignore_specified_properties()
    {
        var mapper = new MapperObjService();
        var origem = new ConfigCryptograph("teste.conf")
        {
            Key = "ChaveSecreta",
            HmacKey = "HMAC123"
        };

        var exclusoes = new List<string> { nameof(ConfigCryptograph.HmacKey) };
        
        var dto = mapper.MapToDto(origem, new ConfigCryptographDto(), exclusoes);
        
        Assert.Equal(origem.Key, dto.Key);
        Assert.Null(dto.HmacKey);
    }
    
    [Fact]
    public void MapToObj_ComOrigemNull_DeveLancarExcecao()
    {
        var mapper = new MapperObjService();
        ConfigCryptographDto dto = null!;
        
        Assert.Throws<ArgumentNullException>(() => 
            mapper.MapToObj(dto, new ConfigCryptograph("teste.conf"))
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