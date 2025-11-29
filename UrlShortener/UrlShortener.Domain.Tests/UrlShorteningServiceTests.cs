using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using UrlShortener.Domain.Services;
using UrlShortener.Infrastructure.Data.Entities;
using UrlShortener.Infrastructure.Data.Repositories;
using Xunit;

namespace UrlShortener.Domain.Tests
{
    public class UrlShorteningServiceTests
    {
        private readonly Mock<IShortenedUrlsRepository> _repositoryMock;
        private readonly UrlShorteningService _service;

        public UrlShorteningServiceTests()
        {
            _repositoryMock = new Mock<IShortenedUrlsRepository>();
            _service = new UrlShorteningService(_repositoryMock.Object);
        }

        [Fact]
        public async Task GenerateShortCode_CreatesAndReturnsNewShortenedUrl_IfNotExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var longUrl = "https://example.com";
            var requestUrl = "https://short.com";
            _repositoryMock.Setup(r => r.GetByLongUrlAndUserIdAsync(longUrl, userId)).ReturnsAsync((ShortenedUrl)null);
            _repositoryMock.Setup(r => r.CodeExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

            ShortenedUrl? added = null;
            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<ShortenedUrl>()))
                .Callback<ShortenedUrl>(s => added = s)
                .Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.GenerateShortCode(longUrl, requestUrl, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(longUrl, result.LongUrl);
            Assert.Equal(userId, result.CreatedBy);
            Assert.False(string.IsNullOrEmpty(result.Code));
            Assert.NotNull(added);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<ShortenedUrl>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Get_ReturnsShortenedUrl_IfExists()
        {
            // Arrange
            var code = "abc123";
            var expected = new ShortenedUrl { Code = code };
            _repositoryMock.Setup(r => r.GetByCodeAsync(code)).ReturnsAsync(expected);

            // Act
            var result = await _service.Get(code);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task GetAll_ReturnsAllShortenedUrls()
        {
            // Arrange
            var urls = new List<ShortenedUrl> { new ShortenedUrl(), new ShortenedUrl() };
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(urls);

            // Act
            var result = await _service.GetAll();

            // Assert
            Assert.Equal(urls, result.ToList());
        }

        [Fact]
        public async Task Delete_RemovesUrl_IfUserIsOwner()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var urlId = Guid.NewGuid();
            var url = new ShortenedUrl { Id = urlId, CreatedBy = userId };
            _repositoryMock.Setup(r => r.GetByIdAsync(urlId)).ReturnsAsync(url);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.Delete(userId, urlId);

            // Assert
            Assert.True(result);
            _repositoryMock.Verify(r => r.Remove(url), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnsFalse_IfUrlNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var urlId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync(urlId)).ReturnsAsync((ShortenedUrl)null);

            // Act
            var result = await _service.Delete(userId, urlId);

            // Assert
            Assert.False(result);
            _repositoryMock.Verify(r => r.Remove(It.IsAny<ShortenedUrl>()), Times.Never);
        }

        [Fact]
        public async Task Delete_ReturnsFalse_IfUserIsNotOwner()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var urlId = Guid.NewGuid();
            var url = new ShortenedUrl { Id = urlId, CreatedBy = Guid.NewGuid() };
            _repositoryMock.Setup(r => r.GetByIdAsync(urlId)).ReturnsAsync(url);

            // Act
            var result = await _service.Delete(userId, urlId);

            // Assert
            Assert.False(result);
            _repositoryMock.Verify(r => r.Remove(It.IsAny<ShortenedUrl>()), Times.Never);
        }
    }
}