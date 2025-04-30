using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using webApiProject.Interfaces;
using webApiProject.Models;
using webApiProject.Services;

namespace webApiProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IService<Users> usersService;

        public LoginController(IService<Users> userService)
        {
            this.usersService = userService;
        }

        [HttpPost]
        [Route("[action]")]
        public ActionResult<String> Login([FromBody] Users user)
        {
            if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Password))
                return BadRequest("Email and Password are required.");

            var existingUser = usersService.Get().FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);
            System.Console.WriteLine(existingUser.Id);
            if (existingUser == null)
                return Unauthorized();

            var claims = new List<Claim> { new Claim("userId", existingUser.Id.ToString()) };
            if (user.Email != "g" || user.Password != "1")
                claims.Add(new Claim("type", "user"));
            else
                claims.Add(new Claim("type", "admin"));
            var token = AuthTokenService.GetToken(claims);
            return new OkObjectResult(AuthTokenService.WriteToken(token));
        }

        // [HttpPost]
        // [Route("[action]")]
        // [Authorize(Policy = "admin")]
        // public IActionResult GenerateBadge([FromBody] Agent Agent)
        // {
        //     var claims = new List<Claim>
        //     {
        //         new Claim("type", "Agent"),
        //         new Claim("ClearanceLevel", Agent.ClearanceLevel.ToString()),
        //     };

        //     var token = AuthTokenService.GetToken(claims);

        //     return new OkObjectResult(AuthTokenService.WriteToken(token));
        // }

    }



}
