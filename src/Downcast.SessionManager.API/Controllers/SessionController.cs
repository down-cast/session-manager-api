using Downcast.SessionManager.Jwt;

using Microsoft.AspNetCore.Mvc;

namespace Downcast.SessionManager.API.Controllers;

[ApiController]
[Route("api/v1/session")]
public class SessionController : ControllerBase
{
    private readonly IJwtManager _jwtManager;
    private readonly ILogger<SessionController> _logger;

    public SessionController(ILogger<SessionController> logger, IJwtManager jwtManager)
    {
        _logger     = logger;
        _jwtManager = jwtManager;
    }

    [HttpPost]
    public string CreateSession(IDictionary<string, object> claims)
    {
        return _jwtManager.GenerateToken(claims);
    }

    [HttpPost("validate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<IDictionary<string, object>> ValidateSession([FromBody] string token)
    {
        return _jwtManager.ValidateToken(token);
    }
}