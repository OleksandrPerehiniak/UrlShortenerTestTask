using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Domain.Models;
using Xunit;

namespace UrlShortener.Domain.Tests
{
    public class LoginModelTests
    {
        [Fact]
        public void LoginModel_CanSetAndGet_Properties()
        {
            var model = new LoginModel
            {
                Email = "user@example.com",
                Password = "pass"
            };
            Assert.Equal("user@example.com", model.Email);
            Assert.Equal("pass", model.Password);
        }
    }
}
