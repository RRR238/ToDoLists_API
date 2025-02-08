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
        Security _security;
        IConfiguration _config;
        public AuthenticationController(UsersRepository usersRepo, Security security, IConfiguration configuration) 
        {
            _usersRepo = usersRepo;
            _security =security;
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


            string hashed_password = _security.HashPassword(user.password);
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
            if (existingUser == null)
                return Unauthorized("User does not exist");

            bool verifiedPassword = _security.VerifyPassword(user.password, existingUser.password);

            if(!verifiedPassword)
                return Unauthorized();

            var tokenToReturn = _security.GetJWTToken(_config, existingUser);
            return Ok(tokenToReturn);
        }
    }
}
