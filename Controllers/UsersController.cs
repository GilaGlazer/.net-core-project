using Microsoft.AspNetCore.Mvc;
using webApiProject.Models;
using webApiProject.Interfaces;
using webApiProject.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace webApiProject.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IService<Users> usersService;
    public UsersController(IService<Users> usersService)
    {
        this.usersService = usersService;
    }

    // [HttpPost]
    // [Route("[action]")]
    // public ActionResult<String> Login([FromBody] Users User)
    // {
    //     // Users? author = UsersTokenService.GetTokenValidationParameters().FirstOrDefault(au => au.Id == loginRequest.Id && au.Name == loginRequest.Name);

    //     if (User.Type != "Admin" ||
    //         User.Password!= "1234" )
    //     {
    //         return Unauthorized();
    //     }

    //     var claims = new List<Claim>
    //         {
    //             new Claim("type", "Admin"),
    //             new Claim("ClearanceLevel", "2"),
    //         };

    //     var token = UsersTokenService.GetToken(claims);

    //     return new OkObjectResult(UsersTokenService.WriteToken(token));
    // }


    // [HttpPost]
    // [Route("[action]")]
    // [Authorize(Policy = "Admin")]
    // public IActionResult GenerateBadge([FromBody] Agent Agent)
    // {
    //     var claims = new List<Claim>
    //         {
    //             new Claim("type", "Agent"),
    //             new Claim("ClearanceLevel", Agent.ClearanceLevel.ToString()),
    //         };

    //     var token = UsersTokenService.GetToken(claims);

    //     return new OkObjectResult(UsersTokenService.WriteToken(token));
    // }
    




    [HttpGet]
    public ActionResult<IEnumerable<Users>> Get()
    {
        System.Console.WriteLine("Get all users");
        return usersService.Get();
    }

    [HttpGet("{id}")]
    public ActionResult<Users> Get(int id)
    {
        var user = usersService.Get(id);
        if (user == null)
            return NotFound();
        return user;
    }

    [HttpPost]
    public ActionResult Post(Users newItem)
    {
        var newId = usersService.Insert(newItem);
        if (newId == -1)
            return BadRequest();
        return CreatedAtAction(nameof(Post), new { Id = newId }, newItem);
    }

    [HttpPut("{id}")]
    public ActionResult Put(int id, Users newItem)
    {
        if (usersService.Update(id, newItem))
            return NoContent();
        return BadRequest();
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        if (usersService.Delete(id))
            return Ok();
        return NotFound();
    }
}
