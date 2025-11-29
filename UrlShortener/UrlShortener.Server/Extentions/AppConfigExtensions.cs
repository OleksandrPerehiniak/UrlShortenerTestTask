using Microsoft.Extensions.Options;
using UrlShortener.Domain.Models;

namespace UrlShortener.Server.Extentions
{
  public static class AppConfigExtensions
  {
    public static IServiceCollection AddAppConfig(
        this IServiceCollection services,
        IConfiguration config)
    {
      services.Configure<AppSettings>(
          config.GetSection("AppSettings"));

      services.AddSingleton(resolver =>
          resolver.GetRequiredService<IOptions<AppSettings>>().Value);

      return services;
    }

    public static WebApplication ConfigureCORS(
        this WebApplication app,
        IConfiguration config)
    {
      app.UseCors(options =>
      options.WithOrigins("http://localhost:4200", "http://localhost:52066")
      .AllowAnyMethod()
      .AllowAnyHeader());
      return app;
    }
  }
}
