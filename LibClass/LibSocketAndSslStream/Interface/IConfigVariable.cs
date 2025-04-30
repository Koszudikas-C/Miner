using LibCryptography.Entities;
using LibDto.Dto;
using LibSocketAndSslStream.Entities;

namespace LibSocketAndSslStream.Interface;

public interface IConfigVariable
{
    ConfigCryptograph GetConfigVariable();
}