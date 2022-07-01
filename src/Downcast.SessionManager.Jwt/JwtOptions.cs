namespace Downcast.SessionManager.Jwt;

public class JwtOptions
{
    public const string OptionsSection = "JwtOptions";
    public TimeSpan Duration { get; set; }
    public string Key { get; set; }
}