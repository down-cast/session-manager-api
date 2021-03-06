using System.Security.Claims;
using System.Text;

using Downcast.Common.Errors;

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

    public string GenerateToken(IDictionary<string, string> claims)
    {
        var handler = new JsonWebTokenHandler();
        var claimsList = claims.Select(pair => new Claim(pair.Key, pair.Value)).ToList();
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_options.Value.Key));
        return handler.CreateToken(new SecurityTokenDescriptor
        {
            Expires            = DateTime.UtcNow.Add(_options.Value.Duration),
            Subject            = new ClaimsIdentity(claimsList),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature),
            Issuer             = _options.Value.Issuer,
            IssuedAt           = DateTime.UtcNow,
            Audience           = _options.Value.Audience
        });
    }

    public async Task<IDictionary<string, object>> ValidateToken(string token)
    {
        try
        {
            JsonWebTokenHandler handler = new();
            TokenValidationResult? validationResponse = await handler.ValidateTokenAsync(
                token, new TokenValidationParameters
                {
                    ValidateIssuer           = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey         = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_options.Value.Key)),
                    ClockSkew                = TimeSpan.Zero,
                    RequireSignedTokens      = true,
                    ValidateAudience         = true,
                    ValidAudience            = _options.Value.Audience,
                    ValidIssuer              = _options.Value.Issuer
                }).ConfigureAwait(false);

            if (validationResponse.IsValid)
            {
                return validationResponse.Claims;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Jwt validation failed");
        }

        throw new DcException(ErrorCodes.InvalidSessionToken);
    }
}