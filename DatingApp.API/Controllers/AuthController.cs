
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTOS;
using DatingApp.API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
      public class AuthController:ControllerBase
    {

          private readonly IAuthRepository _repo ;
          private readonly IConfiguration _config;
        
          
           public AuthController(IAuthRepository repo, IConfiguration config)
             {
                   
                 _repo=repo;
                  _config = config;
             }



           [HttpPost("register")]
           public async Task<IActionResult> Register(UserForRegisterDTO _UserForRegisterDTO)
           {

              _UserForRegisterDTO.Username=_UserForRegisterDTO.Username.ToLower();

              
             if( await _repo.UserExists(_UserForRegisterDTO.Username.ToLower()))
             {
                 return BadRequest("User already exists");

             }   

             var userToCreate = new User
             {
               Username=_UserForRegisterDTO.Username                 
             };

             var result= await _repo.Register(userToCreate,_UserForRegisterDTO.Password);

             return StatusCode(201);

           }


[HttpPost("login")]
public async Task<IActionResult> Login(UserForLoginDTO _UserForLoginDTO)
{

       var userFromRepo = await _repo.Login(_UserForLoginDTO.username.ToLower(), _UserForLoginDTO.password);

       if(userFromRepo == null)
          return Unauthorized();
   
       var claims = new[]
       {
         new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
         new Claim(ClaimTypes.Name, userFromRepo.Username)
       };   

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("AppSettings:Token")));
     // var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super secret key"));

      var creds =new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

      var tokenDescriptor = new SecurityTokenDescriptor
      { 
          Subject = new ClaimsIdentity(claims),
          //24 hour validity
          Expires = DateTime.Now.AddDays(1),
          SigningCredentials = creds
      };
 
      var tokenHandler = new JwtSecurityTokenHandler();

      var token = tokenHandler.CreateToken(tokenDescriptor);

      return Ok(new 
     {
     token=tokenHandler.WriteToken(token)
     });

}



    }
}