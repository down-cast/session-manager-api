namespace Downcast.SessionManager.Jwt;

public interface IJwtManager
{
    string GenerateToken(IDictionary<string, object> claims);
    bool IsTokenValid(string token);
}