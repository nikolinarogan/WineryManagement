using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WineryAPI.Models;

namespace WineryAPI.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(Zaposleni zaposleni)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, zaposleni.Idzap.ToString()),
                new Claim(ClaimTypes.Email, zaposleni.Email),
                new Claim(ClaimTypes.Name, $"{zaposleni.Ime} {zaposleni.Prez}"),
                new Claim(ClaimTypes.Role, zaposleni.Kategorija),
                new Claim("jmbg", zaposleni.Jmbg)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["JwtSettings:Secret"] ?? throw new InvalidOperationException("JWT Secret nije konfigurisan")));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiry = DateTime.UtcNow.AddHours(
                double.Parse(_configuration["JwtSettings:ExpiryInHours"] ?? "24"));

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

