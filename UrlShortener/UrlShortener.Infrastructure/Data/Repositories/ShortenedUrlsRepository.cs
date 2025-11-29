using Microsoft.EntityFrameworkCore;
using UrlShortener.Infrastructure.Data.Context;
using UrlShortener.Infrastructure.Data.Entities;

namespace UrlShortener.Infrastructure.Data.Repositories
{
    public class ShortenedUrlsRepository(UrlShortenerDbContext context) : IShortenedUrlsRepository
    {
        public Task<ShortenedUrl?> GetByLongUrlAndUserIdAsync(string longUrl, Guid userId)
        {
            return context.ShortenedUrls.FirstOrDefaultAsync(x => x.LongUrl == longUrl && x.CreatedBy == userId);
        }

        public async Task AddAsync(ShortenedUrl shortenedUrl)
        {
            await context.ShortenedUrls.AddAsync(shortenedUrl);
        }

        public Task<ShortenedUrl?> GetByCodeAsync(string code)
        {
            return context.ShortenedUrls.FirstOrDefaultAsync(s => s.Code == code);
        }

        public async Task<IEnumerable<ShortenedUrl>> GetAllAsync()
        {
            return await context.ShortenedUrls.ToListAsync();
        }

        public async Task<ShortenedUrl?> GetByIdAsync(Guid id)
        {
            return await context.ShortenedUrls.FindAsync(id);
        }

        public void Remove(ShortenedUrl shortenedUrl)
        {
            context.ShortenedUrls.Remove(shortenedUrl);
        }

        public Task<bool> CodeExistsAsync(string code)
        {
            return context.ShortenedUrls.AnyAsync(s => s.Code == code);
        }

        public Task<int> SaveChangesAsync()
        {
            return context.SaveChangesAsync();
        }
    }
}

