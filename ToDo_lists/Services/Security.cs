using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToDo_lists.Entities;

namespace ToDo_lists.Services
{
    public class Security
    {
        public string HashPassword(string plainPassword)
        {
            // Generates a salt and hashes the password
            return BCrypt.Net.BCrypt.HashPassword(plainPassword);
        }

        // Verify a password
        public bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
        }


        public string GetJWTToken(IConfiguration _config, Users existingUser=null, bool tokenForClient = true)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Authentication:SecretKey"]));

            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claimsForToken = new List<Claim>();

            if (tokenForClient)
            {
                if(existingUser is null)
                    throw new ArgumentNullException(nameof(existingUser), "User cannot be null when generating a client token.");

                claimsForToken.Add(new Claim("sub", existingUser.id.ToString()));
                claimsForToken.Add(new Claim(ClaimTypes.Name, existingUser.name));
                claimsForToken.Add(new Claim(ClaimTypes.Role, "client"));  // Added role for clarity
            }
            else
            {
                claimsForToken.Add(new Claim(ClaimTypes.Role, "internal-service")); // 🔥 Important claim for service-to-service auth
            }

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _config["Authentication:Issuer"],
                audience: _config["Authentication:Audience"],
                claims: claimsForToken,
                expires: tokenForClient ? DateTime.UtcNow.AddHours(1) : DateTime.UtcNow.AddMinutes(1), // 🔥 Shorter expiry for internal token
                signingCredentials: signingCredentials
            );

            var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return tokenToReturn;
        }
    }
}
