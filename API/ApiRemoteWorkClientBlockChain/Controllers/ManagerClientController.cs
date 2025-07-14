using System.Net;
using ApiRemoteWorkClientBlockChain.Interface;
using ApiRemoteWorkClientBlockChain.Interface.Repository;
using Asp.Versioning;
using LibCommunicationStateRemote.Entities;
using LibEntitiesRemote.Entities;
using LibEntitiesRemote.Entities.Client;
using Microsoft.AspNetCore.Mvc;

namespace ApiRemoteWorkClientBlockChain.Controllers;

[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
public class ManagerClientController(IManagerClient managerClient, IClient clientRepository) : ControllerBase
{
    [HttpGet]
    public ActionResult<ApiResponse<ClientInfo>> GetAllClientInfo([FromQuery]int page, int pageSize)
    {
        var apiResponse = managerClient.GetAllClientInfo(page, pageSize);
        return  StatusCode((int)apiResponse.StatusCode, apiResponse);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<Client>>>> GetAllClientAsync([FromQuery]int page, int pageSize,
        CancellationToken cts = default)
    {
        var apiResponse = await clientRepository.GetAllAsync(page, pageSize, cts);
        
        return StatusCode((int)apiResponse.StatusCode, apiResponse);
    }
}
