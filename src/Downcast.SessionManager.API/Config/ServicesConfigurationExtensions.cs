using Downcast.SessionManager.Jwt;

namespace Downcast.SessionManager.API.Config;

public static class ServicesConfigurationExtensions
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.OptionsSection));
        builder.Services.AddSingleton<IJwtManager, JwtManager>();
        return builder;
    }
}