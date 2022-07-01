namespace Downcast.SessionManager.Jwt;

public interface IJwtManager
{
    string GenerateToken(IDictionary<string, object> claims);
    Task<IDictionary<string, object>> ValidateToken(string token);
}