using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Downcast.SessionManager.Jwt;

public class JwtManager : IJwtManager
{
    private readonly ILogger<JwtManager> _logger;
    private readonly IOptions<JwtOptions> _options;

    public JwtManager(ILogger<JwtManager> logger, IOptions<JwtOptions> options)
    {
        _logger  = logger;
        _options = options;
    }

    public string GenerateToken(IDictionary<string, object> claims)
    {
        var handler = new JsonWebTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_options.Value.Key));
        var descriptor = new SecurityTokenDescriptor
        {
            Expires            = DateTime.UtcNow.Add(_options.Value.Duration),
            Claims             = claims,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature)
        };
        return handler.CreateToken(descriptor);
    }

    public bool IsTokenValid(string token)
    {
        throw new NotImplementedException();
    }
}