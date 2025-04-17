using System.Net;
using ApiRemoteWorkClientBlockChain.Interface;
using Asp.Versioning;
using LibCommunicationStatus.Entities;
using LibRemoteAndClient.Entities.Remote.Client;
using Microsoft.AspNetCore.Mvc;

namespace ApiRemoteWorkClientBlockChain.Controllers;

[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
public class ManagerClientV1Controller(IManagerClient managerClient) : ControllerBase
{
    [HttpGet]
    public ActionResult<ApiResponse<ClientInfo>> GetAllClientInfo([FromQuery]int page, int pageSize)
    {
        var apiResponse = managerClient.GetAllClientInfo(page, pageSize);
        return  StatusCode((int)apiResponse.StatusCode, apiResponse);
    }
}