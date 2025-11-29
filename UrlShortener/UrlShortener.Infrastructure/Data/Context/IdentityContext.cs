using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Infrastructure.Data.Entities;

namespace UrlShortener.Infrastructure.Data.Context
{
    public class IdentityContext(DbContextOptions<IdentityContext> options) : IdentityDbContext(options)
    {
        public DbSet<AppUser>? AppUsers { get; set; }
    }
}
