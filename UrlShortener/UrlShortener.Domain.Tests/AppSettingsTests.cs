using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Domain.Models;
using Xunit;

namespace UrlShortener.Domain.Tests
{
    public class AppSettingsTests
    {
        [Fact]
        public void AppSettings_CanSetAndGet_JWTSecret()
        {
            var settings = new AppSettings { JWTSecret = "mysecret" };
            Assert.Equal("mysecret", settings.JWTSecret);
        }
    }
}
