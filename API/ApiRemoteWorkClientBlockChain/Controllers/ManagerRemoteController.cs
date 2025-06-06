using System.Net;
using ApiRemoteWorkClientBlockChain.Interface;
using Asp.Versioning;
using LibCommunicationStateRemote.Entities;
using LibSocketAndSslStreamRemote.Entities;
using LibSocketAndSslStreamRemote.Entities.Enum;
using Microsoft.AspNetCore.Mvc;

namespace ApiRemoteWorkClientBlockChain.Controllers;

[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
public class ManagerRemoteController(IManagerConnection managerConnection) : ControllerBase
{
  [HttpPost]
  public async Task<ActionResult<ApiResponse<string>>> StartRemoteAsync([FromBody] ConnectionConfig connectionConfig,
    TypeAuthMode typeAuthMode,
    CancellationToken cts = default)
  {
    var apiResponse = await managerConnection.InitializeAsync(connectionConfig, typeAuthMode, cts);

    return StatusCode((int)apiResponse.StatusCode, apiResponse);
  }
}
