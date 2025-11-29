using UrlShortener.Infrastructure.Data.Entities;

namespace UrlShortener.Infrastructure.Data.Repositories
{
    public interface IShortenedUrlsRepository
    {
        Task<ShortenedUrl?> GetByLongUrlAndUserIdAsync(string longUrl, Guid userId);
        Task AddAsync(ShortenedUrl shortenedUrl);
        Task<ShortenedUrl?> GetByCodeAsync(string code);
        Task<IEnumerable<ShortenedUrl>> GetAllAsync();
        Task<ShortenedUrl?> GetByIdAsync(Guid id);
        void Remove(ShortenedUrl shortenedUrl);
        Task<bool> CodeExistsAsync(string code);
        Task<int> SaveChangesAsync();
    }
}

