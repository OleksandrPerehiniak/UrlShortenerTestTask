using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using UrlShortener.Domain.Models;
using UrlShortener.Infrastructure.Data.Entities;
using UrlShortener.Server.Controllers;
using Xunit;
namespace UrlShortener.Server.Tests
{
    public class IdentityControllerTests
    {
        private readonly Mock<UserManager<AppUser>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly IOptions<AppSettings> _appSettings;

        public IdentityControllerTests()
        {
            var userStoreMock = new Mock<IUserStore<AppUser>>();
            _userManagerMock = new Mock<UserManager<AppUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStoreMock.Object, null, null, null, null);

            _appSettings = Options.Create(new AppSettings { JWTSecret = "GiveASecretKeyHavingAtleast32Characters" });
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenRegistrationSucceeds()
        {
            // Arrange
            var registrationModel = new UserRegistrationModel
            {
                Email = "test@example.com",
                Password = "Password123!",
                FullName = "Test User",
                Role = "User"
            };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), registrationModel.Password))
                .ReturnsAsync(IdentityResult.Success);
            _roleManagerMock.Setup(x => x.RoleExistsAsync("User")).ReturnsAsync(true);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<AppUser>(), "User"))
                .ReturnsAsync(IdentityResult.Success);

            var controller = new IdentityController(_userManagerMock.Object, _roleManagerMock.Object, _appSettings);

            // Act
            var result = await controller.Register(registrationModel);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenRegistrationFails()
        {
            // Arrange
            var registrationModel = new UserRegistrationModel
            {
                Email = "fail@example.com",
                Password = "Password123!",
                FullName = "Fail User"
            };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), registrationModel.Password))
                .ReturnsAsync(IdentityResult.Failed());

            var controller = new IdentityController(_userManagerMock.Object, _roleManagerMock.Object, _appSettings);

            // Act
            var result = await controller.Register(registrationModel);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task SignIn_ReturnsOk_WithToken_WhenCredentialsAreValid()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                Email = "test@example.com",
                Password = "Password123!"
            };

            var user = new AppUser { Id = Guid.NewGuid().ToString(), Email = loginModel.Email, UserName = loginModel.Email };
            _userManagerMock.Setup(x => x.FindByEmailAsync(loginModel.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, loginModel.Password)).ReturnsAsync(true);
            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new[] { "User" });

            var controller = new IdentityController(_userManagerMock.Object, _roleManagerMock.Object, _appSettings);

            // Act
            var result = await controller.SignIn(loginModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            Assert.Contains("token", okResult.Value.ToString());
        }

        [Fact]
        public async Task SignIn_ReturnsBadRequest_WhenCredentialsAreInvalid()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                Email = "wrong@example.com",
                Password = "WrongPassword!"
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(loginModel.Email)).ReturnsAsync((AppUser)null);

            var controller = new IdentityController(_userManagerMock.Object, _roleManagerMock.Object, _appSettings);

            // Act
            var result = await controller.SignIn(loginModel);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("incorrect", badRequest.Value.ToString());
        }
    }
}