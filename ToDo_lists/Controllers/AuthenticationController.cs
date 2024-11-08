using Microsoft.AspNetCore.Mvc;
using ToDo_lists.Models;
using Microsoft.AspNetCore.Authorization;
using ToDo_lists.Services;
using ToDo_lists.Entities;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace ToDo_lists.Controllers
{
    [ApiController]
    public class AuthenticationController: ControllerBase
    {

        UsersRepository _usersRepo;
        PasswordHash _passwordHash;
        IConfiguration _config;
        public AuthenticationController(UsersRepository usersRepo, PasswordHash passwordHash, IConfiguration configuration) 
        {
            _usersRepo = usersRepo;
            _passwordHash = passwordHash;
            _config = configuration;
        }

        [HttpPost("registration")]
        public async Task<IActionResult> Registration([FromBody] UsersModel user)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            Users existingUser = await _usersRepo.GetItemByName(user.name);

            if (existingUser != null)
                return Conflict(new { message = "User name already exists." });


            string hashed_password = _passwordHash.HashPassword(user.password);
            Users newUser = new Users { name = user.name, password = hashed_password };
            _usersRepo.AddUser(newUser);

          
            return Created(string.Empty, new { message = "Registration successful" });
            
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UsersModel user)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            Users existingUser = await _usersRepo.GetItemByName(user.name);
            bool verifiedPassword = _passwordHash.VerifyPassword(user.password, existingUser.password);

            if(!verifiedPassword)
                return Unauthorized();

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Authentication:SecretKey"]));

            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", existingUser.id.ToString()));
            claimsForToken.Add(new Claim(ClaimTypes.Name, existingUser.name));

            var jwtSecurityToken = new JwtSecurityToken(_config["Authentication:Issuer"],
                                                        _config["Authentication:Audience"],
                                                        claimsForToken,
                                                        DateTime.UtcNow,
                                                        DateTime.UtcNow.AddHours(1),
                                                        signingCredentials);

            var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return Ok(tokenToReturn);
        }
    }
}
