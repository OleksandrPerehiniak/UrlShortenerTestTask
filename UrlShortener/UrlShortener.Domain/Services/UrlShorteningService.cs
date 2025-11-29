using UrlShortener.Infrastructure.Data.Entities;
using UrlShortener.Infrastructure.Data.Repositories;

namespace UrlShortener.Domain.Services
{
    public class UrlShorteningService(IShortenedUrlsRepository repository) : IUrlShorteningService
    {
        private const int NumberOfCharacters = 7;
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private readonly Random _random = new();

        public async Task<ShortenedUrl> GenerateShortCode(string requestUrl, string longUrl, Guid userId)
        {
            var code = await repository.GetByLongUrlAndUserIdAsync(longUrl, userId);
            if (code is not null)
            {
                return code;
            }

            var shortUrl = await GetShortenUrl();

            var shortenedUrl = new ShortenedUrl
            {
                Id = Guid.NewGuid(),
                LongUrl = requestUrl,
                Code = shortUrl,
                ShortUrl = $"{longUrl}/{shortUrl}",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            await repository.AddAsync(shortenedUrl);
            await repository.SaveChangesAsync();

            return shortenedUrl;
        }

        public Task<ShortenedUrl?> Get(string code)
        {
            return repository.GetByCodeAsync(code);
        }

        public async Task<IEnumerable<ShortenedUrl?>> GetAll()
        {
            return await repository.GetAllAsync();
        }

        public async Task<bool> Delete(Guid userId, Guid id)
        {
            var urlToDelete = await repository.GetByIdAsync(id);
            if (urlToDelete == null)
            {
                return false;
            }

            if (userId == urlToDelete.CreatedBy)
            {
                repository.Remove(urlToDelete);
                await repository.SaveChangesAsync();
                return true;
            }

            return false;
        }

        private async Task<string> GetShortenUrl()
        {
            while (true)
            {
                var codeChars = new char[NumberOfCharacters];
                for (int i = 0; i < NumberOfCharacters; i++)
                {
                    var randomIndex = _random.Next(Alphabet.Length - 1);
                    codeChars[i] = Alphabet[randomIndex];
                }
                var code = new string(codeChars);

                if (!await repository.CodeExistsAsync(code))
                {
                    return code;
                }
            }
        }
    }
}

