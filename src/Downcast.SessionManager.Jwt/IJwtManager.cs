using Downcast.SessionManager.Jwt.Model;

namespace Downcast.SessionManager.Jwt;

public interface IJwtManager
{
    TokenResult GenerateToken(IDictionary<string, object> claims);
    Task<IDictionary<string, object>> ValidateToken(string token);
}