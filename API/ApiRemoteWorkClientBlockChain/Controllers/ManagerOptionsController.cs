using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiRemoteWorkClientBlockChain.Controllers;

[ApiVersion(1)]
[Route("api/{version:apiVersion}/[controller]/[action]")]
[ApiController]
public class ManagerOptionsController : ControllerBase
{
}