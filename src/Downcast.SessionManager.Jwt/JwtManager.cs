using System.Security.Claims;
using System.Text;
using System.Text.Json;

using Downcast.Common.Errors;
using Downcast.SessionManager.Jwt.Model;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Downcast.SessionManager.Jwt;

public class JwtManager : IJwtManager
{
    private readonly JsonWebTokenHandler _handler = new();
    private readonly ILogger<JwtManager> _logger;
    private readonly IOptions<JwtOptions> _options;

    public JwtManager(ILogger<JwtManager> logger, IOptions<JwtOptions> options)
    {
        _logger  = logger;
        _options = options;
    }

    public TokenResult GenerateToken(IDictionary<string, object> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_options.Value.Key));
        DateTime expirationDate = DateTime.UtcNow.Add(_options.Value.Duration);

        IEnumerable<Claim> claimList = GetClaims(claims);
        string token = _handler.CreateToken(new SecurityTokenDescriptor
        {
            Expires            = expirationDate,
            Subject            = new ClaimsIdentity(claimList),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature),
            Issuer             = _options.Value.Issuer,
            IssuedAt           = DateTime.UtcNow,
            Audience           = _options.Value.Audience
        });

        return new TokenResult
        {
            Token          = token,
            ExpirationDate = expirationDate
        };
    }


    public async Task ValidateToken(string token)
    {
        try
        {
            TokenValidationResult validationResponse = await _handler.ValidateTokenAsync(
                token,
                new TokenValidationParameters
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

            if (!validationResponse.IsValid)
            {
                throw new DcException(ErrorCodes.InvalidSessionToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Jwt validation failed");
            throw new DcException(ErrorCodes.InvalidSessionToken);
        }
    }

    private IEnumerable<Claim> GetClaims(IDictionary<string, object> claims)
    {
        string? tempToken = _handler.CreateToken(JsonSerializer.Serialize(claims));
        JsonWebToken? readToken = _handler.ReadJsonWebToken(tempToken);
        return readToken.Claims;
    }
}