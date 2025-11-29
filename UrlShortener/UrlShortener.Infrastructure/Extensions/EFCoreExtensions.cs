using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Infrastructure.Data.Context;

namespace UrlShortener.Server.Extentions
{
    public static class EFCoreExtensions
    {
        public static IServiceCollection InjectDbContext(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddDbContext<IdentityContext>(options =>
                     options.UseSqlServer(config.GetConnectionString("Database")));
            services.AddDbContext<UrlShortenerDbContext>(options =>
                     options.UseSqlServer(config.GetConnectionString("Database")));

            return services;
        }
    }
}
