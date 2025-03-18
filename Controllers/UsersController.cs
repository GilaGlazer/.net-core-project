using Microsoft.AspNetCore.Mvc;
using webApiProject.Models;
using webApiProject.Interfaces;

namespace webApiProject.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private IUsersService usersService;

    public UsersController(IUsersService usersService)
    {
        this.usersService = usersService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Users>> Get()
    {
        return usersService.Get();
    }

    [HttpGet("{email}")]
    public ActionResult<Users> Get(string email)
    {
        var user = usersService.Get(email);
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
        return CreatedAtAction(nameof(Post), new { Id = newId },newItem);
    }

    [HttpPut("{email}")]
    public ActionResult Put(string email, Users newItem)
    {
        if (usersService.Update(email, newItem))
            return NoContent();
        return BadRequest();
    }

    [HttpDelete("{email}")]
    public ActionResult Delete(string email)
    {
        if (usersService.Delete(email))
            return Ok();
        return NotFound();
    }
}
