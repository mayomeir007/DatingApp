using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Controllers.Models;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegiaterDto)
        {
            //validate request
            //if(!ModelState.IsValid)
            //    return BadRequest(ModelState);

            userForRegiaterDto.Username = userForRegiaterDto.Username.ToLower();

            if(await _repo.UserExists(userForRegiaterDto.Username))
                return BadRequest("Username already exists");

                var UserToCreate = new User();
                UserToCreate.Username = userForRegiaterDto.Username;

            var CreatedUser = await _repo.Register(UserToCreate, userForRegiaterDto.Password);

            return StatusCode(201);
        } 

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            if(userFromRepo == null)
                return null;

            Claim claim1 = new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString());
            Claim claim2 = new Claim(ClaimTypes.Name, userFromRepo.Username);

            Claim[] claims = new Claim[] {claim1,claim2};

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokensDescriptor = new SecurityTokenDescriptor();

            tokensDescriptor.Subject = new ClaimsIdentity(claims);
            tokensDescriptor.Expires = DateTime.Now.AddDays(1);
            tokensDescriptor.SigningCredentials = creds;

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokensDescriptor);

            //var aString = tokenHandler.WriteToken(token);
            //return Ok(new {token = tokenHandler.WriteToken(token)});
            return Ok(tokenHandler.WriteToken(token));
        }
    }
}