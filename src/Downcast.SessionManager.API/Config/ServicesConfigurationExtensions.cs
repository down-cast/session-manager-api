using Downcast.SessionManager.Jwt;

namespace Downcast.SessionManager.API.Config;

public static class ServicesConfigurationExtensions
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddOptions<JwtOptions>()
            .Bind(builder.Configuration.GetSection(JwtOptions.OptionsSection))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddSingleton<IJwtManager, JwtManager>();
        return builder;
    }
}