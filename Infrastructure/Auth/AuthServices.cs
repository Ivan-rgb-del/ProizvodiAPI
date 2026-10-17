using Microsoft.IdentityModel.Tokens;
using ProizvodiAPI.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static ProizvodiAPI.Application.DTOs.AuthDTO;

namespace ProizvodiAPI.Infrastructure.Auth
{
    public class AuthServices : IAuthServices
    {
        private readonly IConfiguration _configuration;

        public AuthServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private static List<(string KorisnickoIme, string Lozinka, string Rola)> _korisnici = new()
        {
            ("admin", "admin123", "Admin"),
            ("korisnik", "korisnik123", "Korisnik")
        };

        public TokenResponseDTO? Login(LoginDTO dto)
        {
            var korisnik = _korisnici.FirstOrDefault(k =>
                k.KorisnickoIme == dto.KorisnickoIme &&
                k.Lozinka == dto.Lozinka
            );

            if (korisnik == default) return null;

            var token = GenerisiToken(korisnik.KorisnickoIme, korisnik.Rola);
            return token;
        }

        private TokenResponseDTO GenerisiToken(string korisnickoIme, string rola)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var kredencijali = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiry = DateTime.Now.AddMinutes(
                Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"]));

            // Claims - podaci koji se čuvaju u tokenu
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, korisnickoIme),
                new Claim(ClaimTypes.Role, rola),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: kredencijali
            );

            return new TokenResponseDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiry = expiry
            };
        }
    }
}
