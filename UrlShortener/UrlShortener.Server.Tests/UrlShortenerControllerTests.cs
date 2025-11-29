using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using UrlShortener.Domain.Models;
using UrlShortener.Domain.Services;
using UrlShortener.Infrastructure.Data.Entities;
using UrlShortener.Server.Controllers;
using Xunit;

namespace UrlShortener.Server.Tests
{
    public class UrlShortenerControllerTests
    {
        private readonly Mock<IUrlShorteningService> _serviceMock;
        private readonly UrlShortenerController _controller;

        public UrlShortenerControllerTests()
        {
            _serviceMock = new Mock<IUrlShorteningService>();
            _controller = new UrlShortenerController(_serviceMock.Object);

            // Setup HttpContext with a user claim for userID
            var userId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("userID", userId.ToString())
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task ShortenUrl_ReturnsBadRequest_ForInvalidUrl()
        {
            // Arrange
            var request = new ShortenUrlRequest { Url = "not-a-valid-url" };

            // Act
            var result = await _controller.ShortenUrl(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid URL format.", badRequest.Value);
        }

        [Fact]
        public async Task ShortenUrl_ReturnsOk_ForValidUrl()
        {
            // Arrange
            var request = new ShortenUrlRequest { Url = "https://example.com" };
            var shortened = new ShortenedUrl { Id = Guid.NewGuid(), LongUrl = request.Url, ShortUrl = "https://short/abc", Code = "abc" };
            _serviceMock.Setup(s => s.GenerateShortCode(
                request.Url, It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(shortened);

            // Act
            var result = await _controller.ShortenUrl(request);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(shortened, ok.Value);
        }

        [Fact]
        public async Task GetAllUrls_ReturnsOkWithUrls()
        {
            // Arrange
            var urls = new List<ShortenedUrl> { new ShortenedUrl(), new ShortenedUrl() };
            _serviceMock.Setup(s => s.GetAll()).ReturnsAsync(urls);

            // Act
            var result = await _controller.GetAllUrls();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(urls, ok.Value);
        }

        [Fact]
        public async Task GetUrlInfo_ReturnsOk_WhenFound()
        {
            // Arrange
            var code = "abc";
            var url = new ShortenedUrl { Code = code };
            _serviceMock.Setup(s => s.Get(code)).ReturnsAsync(url);

            // Act
            var result = await _controller.GetUrlInfo(code);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(url, ok.Value);
        }

        [Fact]
        public async Task GetUrlInfo_ReturnsNotFound_WhenMissing()
        {
            // Arrange
            var code = "missing";
            _serviceMock.Setup(s => s.Get(code)).ReturnsAsync((ShortenedUrl)null);

            // Act
            var result = await _controller.GetUrlInfo(code);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Shortened URL not found.", notFound.Value);
        }

        [Fact]
        public async Task DeleteUrl_ReturnsNoContent_WhenDeleted()
        {
            // Arrange
            var id = Guid.NewGuid();
            _serviceMock.Setup(s => s.Delete(It.IsAny<Guid>(), id)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteUrl(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteUrl_ReturnsNotFound_WhenNotDeleted()
        {
            // Arrange
            var id = Guid.NewGuid();
            _serviceMock.Setup(s => s.Delete(It.IsAny<Guid>(), id)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteUrl(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}