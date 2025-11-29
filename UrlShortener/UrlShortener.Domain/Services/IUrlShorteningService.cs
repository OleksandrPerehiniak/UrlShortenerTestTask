using UrlShortener.Infrastructure.Data.Entities;

namespace UrlShortener.Domain.Services
{
    public interface IUrlShorteningService
    {
        Task<ShortenedUrl> GenerateShortCode(string requestUrl, string longUrl, Guid userId);
        Task<ShortenedUrl?> Get(string code);
        Task<IEnumerable<ShortenedUrl?>> GetAll();
        Task<bool> Delete(Guid userId, Guid id);
    }
}
