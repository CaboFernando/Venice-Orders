using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace VeniceOrders.API.Controllers
{
    [ApiController]
    [Route("auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IConfiguration _cfg;

        public AuthController(IConfiguration cfg)
        {
            _cfg = cfg;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var token = GenerateToken(request.Username);

            return Ok(new { token });
        }

        private string GenerateToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _cfg["Jwt:Issuer"],
                audience: _cfg["Jwt:Audience"],
                claims: new[] { new Claim(ClaimTypes.Name, username) },
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public sealed record LoginRequest(string Username, string Password);
    }
}


