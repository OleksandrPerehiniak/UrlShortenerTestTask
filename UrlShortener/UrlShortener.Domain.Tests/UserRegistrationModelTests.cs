using UrlShortener.Domain.Models;
using Xunit;

namespace UrlShortener.Domain.Tests
{
    public class UserRegistrationModelTests
    {
        [Fact]
        public void UserRegistrationModel_CanSetAndGet_Properties()
        {
            var model = new UserRegistrationModel
            {
                Email = "reg@example.com",
                Password = "regpass",
                FullName = "Test User",
                Role = "Admin"
            };
            Assert.Equal("reg@example.com", model.Email);
            Assert.Equal("regpass", model.Password);
            Assert.Equal("Test User", model.FullName);
            Assert.Equal("Admin", model.Role);
        }
    }
}