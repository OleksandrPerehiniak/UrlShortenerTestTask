using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Domain.Models;
using UrlShortener.Domain.Services;

namespace UrlShortener.Server.Controllers
{
    [Route("api/shortener")]
    [ApiController]
    [Authorize]
    public class UrlShortenerController(IUrlShorteningService urlShorteningService) : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ShortenUrl([FromBody] ShortenUrlRequest request)
        {
            if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
            {
                return BadRequest("Invalid URL format.");
            }

            Guid guid = GetUserGuid();

            var shortenedUrl = await urlShorteningService.GenerateShortCode(request.Url, $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}", guid);

            return Ok(shortenedUrl);
        }

        [HttpGet("urls")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllUrls()
        {
            var urls = await urlShorteningService.GetAll();
            return Ok(urls);
        }

        [HttpGet("info/{code}")]
        [Authorize]
        public async Task<IActionResult> GetUrlInfo([FromRoute] string code)
        {
            var shortenedUrl = await urlShorteningService.Get(code);

            if (shortenedUrl == null)
            {
                return NotFound("Shortened URL not found.");
            }
            return Ok(shortenedUrl);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUrl(Guid id)
        {
            
            var result = await urlShorteningService.Delete(GetUserGuid(), id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        private Guid GetUserGuid()
        {
            var userId = HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "userID")?.Value;
            var guid = Guid.Parse(userId!);
            return guid;
        }
    }
}
