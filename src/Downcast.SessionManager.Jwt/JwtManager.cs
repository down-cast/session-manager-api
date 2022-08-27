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
        string token = _handler.CreateToken(new SecurityTokenDescriptor
        {
            Expires            = expirationDate,
            Subject            = new ClaimsIdentity(GetClaimsIdentity(claims)),
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

    public ClaimsIdentity GetClaimsIdentity(IDictionary<string, object> claims)
    {
        return new ClaimsIdentity(claims.SelectMany(pair => GetClaims(pair.Key, pair.Value)));
    }


    private IEnumerable<Claim> GetClaims(IDictionary<string, object> claims)
    {
        var claimList = new List<Claim>();
        foreach ((string? claimName, object? value) in claims)
        {
            claimList.AddRange(GetClaims(claimName, value));
        }

        return claimList;
    }


    private IEnumerable<Claim> GetClaims(string claimName, object value)
    {
        var jsonElement = (JsonElement)value;
        object? realValue = GetValueFromJsonElement(jsonElement);
        return realValue switch
        {
            null => Enumerable.Empty<Claim>(),
            IEnumerable<object> enumerable => enumerable.SelectMany(val => GetClaims(claimName, val)),
            _ => new[] { new Claim(claimName, realValue.ToString() ?? "", GetClaimValueType(realValue)) }
        };
    }


    private static object? GetValueFromJsonElement(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Array:
                return element.Deserialize<IEnumerable<object>>();
            case JsonValueKind.String:
                return element.Deserialize<string>();
            case JsonValueKind.Number:
                return element.Deserialize<double>();
            case JsonValueKind.True or JsonValueKind.False:
                return element.Deserialize<bool>();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static string GetClaimValueType(object value)
    {
        return value switch
        {
            bool => ClaimValueTypes.Boolean,
            int => ClaimValueTypes.Integer,
            long => ClaimValueTypes.Integer64,
            double or float => ClaimValueTypes.Double,
            string => ClaimValueTypes.String,
            DateTime => ClaimValueTypes.Date,
            _ => throw new DcException(ErrorCodes.BadRequest, $"Unsupported claim value type: {value.GetType()}")
        };
    }
}