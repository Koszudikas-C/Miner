using ApiRemoteWorkClientBlockChain.Interface;
using LibRemoteAndClient.Entities.Remote.Client;
using LibSocket.Entities;
using LibSsl.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ApiRemoteWorkClientBlockChain.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ManagerRemoteController(IManagerConnection managerConnection) : ControllerBase
{
    [HttpPost("[action]")]
    public async Task<ActionResult> StartRemoteAsync([FromBody]ConnectionConfig connectionConfig,
        CancellationToken cts = default)
    {
        await managerConnection.InitializeAsync(connectionConfig, cts);

        return Ok(new { message = "Remote initialized successfully" });
    }

    [HttpGet("[action]")]
    public ActionResult<IReadOnlyCollection<ClientInfo>> GetClient()
    {
        return Ok(managerConnection.GetClientLast());
    }

}