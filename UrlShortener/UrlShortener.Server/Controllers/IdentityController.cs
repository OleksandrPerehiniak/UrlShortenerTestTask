using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UrlShortener.Domain.Models;
using UrlShortener.Infrastructure.Data.Entities;

namespace UrlShortener.Server.Controllers
{
    [Route("api/identity")]
    [ApiController]
    public class IdentityController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<AppSettings> appSettings) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationModel userRegistrationModel)
        {
            var user = new AppUser()
            {
                FullName = userRegistrationModel.FullName,
                UserName = userRegistrationModel.Email,
                Email = userRegistrationModel.Email

            };
            var result = await userManager.CreateAsync(user, userRegistrationModel.Password);

            if (result.Succeeded)
            {
                var role = string.IsNullOrEmpty(userRegistrationModel.Role) ? "User" : userRegistrationModel.Role;

                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
                await userManager.AddToRoleAsync(user, role);

                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] LoginModel loginModel)
        {
            var user = await userManager.FindByEmailAsync(loginModel.Email);
            if (user != null && await userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                var roles = await userManager.GetRolesAsync(user);
                var signInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Value.JWTSecret!));

                ClaimsIdentity claims = new(
                [
                  new Claim("userID",user.Id.ToString()),
                  new Claim(ClaimTypes.Role,roles.First()),
                ]);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Ok(new { token });
            }

            return BadRequest(new { message = "Username or password is incorrect." });
        }
    }
}
