using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PersonalHealthRecordManagement.Models;

namespace PersonalHealthRecordManagement.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _configuration;


        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public string CreateToken(ApplicationUser user, IList<string> roles)
        {
            var jwtConfig = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtConfig["Key"]!);


            var claims = new List<Claim>
        {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
        new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
        new Claim("uid", user.Id)
        };


            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }


            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            
            if (!int.TryParse(jwtConfig["ExpiresMinutes"], out var expiresMinutes) || expiresMinutes <= 0)
            {
                expiresMinutes = 2880; // Default to 48 hours if invalid
            }
            var expires = DateTime.UtcNow.AddMinutes(expiresMinutes);


            var token = new JwtSecurityToken(
            issuer: jwtConfig["Issuer"],
            audience: jwtConfig["Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
            );


            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
