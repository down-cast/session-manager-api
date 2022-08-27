namespace Downcast.SessionManager.Jwt.Model;

public class TokenResult
{
    public string Token { get; set; } = null!;
    public DateTime ExpirationDate { get; set; }
}