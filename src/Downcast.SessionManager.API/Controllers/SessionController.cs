using Downcast.SessionManager.Jwt;
using Downcast.SessionManager.Jwt.Model;

using Microsoft.AspNetCore.Mvc;

namespace Downcast.SessionManager.API.Controllers;

[ApiController]
[Route("api/v1/session")]
public class SessionController : ControllerBase
{
    private readonly IJwtManager _jwtManager;

    public SessionController(IJwtManager jwtManager)
    {
        _jwtManager = jwtManager;
    }

    [HttpPost]
    public TokenResult CreateSession(IDictionary<string, string> claims)
    {
        return _jwtManager.GenerateToken(claims);
    }

    [HttpPost("validate")]
    public Task<IDictionary<string, object>> ValidateSession([FromBody] string token)
    {
        return _jwtManager.ValidateToken(token);
    }
}