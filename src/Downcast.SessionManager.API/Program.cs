using Downcast.Common.Logging;
using Downcast.SessionManager.API.Config;

using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.ConfigureServices();

builder.ConfigureSerilog();

WebApplication app = builder.Build();

app.Environment.IsDevelopment();
app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.UseForwardedHeaders();
app.UseSerilogRequestLogging(options =>
{
	options.EnrichDiagnosticContext = (context, httpContext) =>
	{
		context.Set("client_ip", httpContext.Connection.RemoteIpAddress);
	};
});

app.Run();