using Microsoft.EntityFrameworkCore;
using UrlShortener.Infrastructure.Data.Entities;

namespace UrlShortener.Infrastructure.Data.Context
{
    public class UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options) : DbContext(options) 
    {
        private const int NumberOfCharacters = 7;

        public DbSet<ShortenedUrl> ShortenedUrls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShortenedUrl>(builder => 
            { 
                builder.Property(e => e.Code).HasMaxLength(NumberOfCharacters);
                builder.HasIndex(e => e.Code).IsUnique();
            });
        }

    }
}
