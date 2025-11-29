using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Domain.Models;
using Xunit;

namespace UrlShortener.Domain.Tests
{
    public class ShortenUrlRequestTests
    {
        [Fact]
        public void ShortenUrlRequest_DefaultUrl_IsEmptyString()
        {
            var req = new ShortenUrlRequest();
            Assert.Equal(string.Empty, req.Url);
        }

        [Fact]
        public void ShortenUrlRequest_CanSetAndGet_Url()
        {
            var req = new ShortenUrlRequest { Url = "https://test.com" };
            Assert.Equal("https://test.com", req.Url);
        }
    }
}
