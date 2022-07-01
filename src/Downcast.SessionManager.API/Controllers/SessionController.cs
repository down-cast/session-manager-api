using System.Net;

using Downcast.SessionManager.Jwt;

using Microsoft.AspNetCore.Mvc;

namespace Downcast.SessionManager.API.Controllers;

[ApiController]
[Route("api/v1/session")]
public class SessionController : ControllerBase
{
    private readonly ILogger<SessionController> _logger;
    private readonly IJwtManager _jwtManager;

    public SessionController(ILogger<SessionController> logger, IJwtManager jwtManager)
    {
        this._logger     = logger;
        _jwtManager = jwtManager;
    }

    [HttpPost]
    public string CreateSession(IDictionary<string, object> claims)
    {
        return _jwtManager.GenerateToken(claims);
    }

    [HttpPost("validate")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public ActionResult ValidateSession(IDictionary<string, object> claims)
    {
        _logger.LogInformation("Received information");
        return Ok(claims);
    }
}