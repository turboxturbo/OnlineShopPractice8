using System.Security.Claims;
using OnlineShop.Requests;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace OnlineShop.UniversalMethods
{
    public class JWTGenerator
    {
        public readonly string secretkey;
        public JWTGenerator(IConfiguration configuration)
        {
            secretkey = configuration["JWT:Key"] ?? throw new Exception("JWT is not found");
        }
        public string GenerateToken(LoginPassword user)
        {
            var claims = new[]
            {
                   new Claim("UserId", user.IdUser.ToString()),
                   new Claim("RoleId", user.IdRole.ToString()),
                   new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                   new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToString()),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretkey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(claims: claims, signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
            
        }
    }
}
