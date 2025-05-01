// using Microsoft.AspNetCore.Mvc;
// using webApiProject.Models;
// using webApiProject.Interfaces;
// using webApiProject.Services;
// using System.Security.Claims;
// using Microsoft.AspNetCore.Authorization;

// namespace webApiProject.Controllers;

// [ApiController]
// [Route("[controller]")]
// public class UsersController : ControllerBase
// {
//     private readonly UsersServiceJson usersService;
//     public UsersController(UsersServiceJson usersService)
//     {
//         System.Console.WriteLine("in Ctor UsersController");
//         this.usersService = usersService;
//     }

//     [HttpGet]
//     [Authorize(Policy = "admin")]
//     public ActionResult<IEnumerable<Users>> Get()
//     {
//         System.Console.WriteLine("in Get controller");

//         return usersService.Get();
//     }

//     [HttpGet("{id}")]
//     [Authorize(Policy = "user")]
//     public ActionResult<Users> Get(int id)
//     {
//         System.Console.WriteLine("in Get whit id controller");

//         var user = usersService.GetMyUser();
//         if (user == null)
//             return NotFound();
//         return user;
//     }

//     [HttpPost]
//     [Authorize(Policy = "admin")]

//     public ActionResult Post(Users newItem)
//     {
//         System.Console.WriteLine("in post controller");

//         var newId = usersService.Insert(newItem);
//         if (newId == -1)
//             return BadRequest();
//         return CreatedAtAction(nameof(Post), new { Id = newId }, newItem);
//     }

//     [HttpPut("{id}")]
//     [Authorize(Policy = "admin")]

//     public ActionResult Put(int id, Users newItem)
//     {
//         System.Console.WriteLine("in Put controller");

//         if (usersService.Update(id, newItem))
//             return NoContent();
//         return BadRequest();
//     }

//     [HttpDelete("{id}")]
//     [Authorize(Policy = "admin")]

//     public ActionResult Delete(int id)
//     {
//         System.Console.WriteLine("in Delete controller");
//         if (usersService.Delete(id))
//             return Ok();
//         return NotFound();
//     }
// }
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
    private readonly UsersServiceJson usersService;
    public UsersController(UsersServiceJson usersService)
    {
        System.Console.WriteLine("in Ctor UsersController");
        this.usersService = usersService;
    }

    [HttpGet]
    [Authorize(Policy = "admin")]
    public ActionResult<IEnumerable<Users>> Get()
    {
        System.Console.WriteLine("in Get controller");

        return usersService.Get();
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "user")]
    public ActionResult<Users> Get(int id)
    {
        System.Console.WriteLine("in Get whit id controller");

        var user = usersService.GetMyUser();
        if (user == null)
            return NotFound();
        return user;
    }

    [HttpPost]
    [Authorize(Policy = "admin")]

    public ActionResult Post(Users newItem)
    {
        System.Console.WriteLine("in post controller");

        var newId = usersService.Insert(newItem);
        if (newId == -1)
            return BadRequest();
        return CreatedAtAction(nameof(Post), new { Id = newId }, newItem);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "admin")]

    public ActionResult Put(int id, Users newItem)
    {
        System.Console.WriteLine("in Put controller");

        if (usersService.Update(id, newItem))
            return NoContent();
        return BadRequest();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "admin")]

    public ActionResult Delete(int id)
    {
        System.Console.WriteLine("in Delete controller");
        if (usersService.Delete(id))
            return Ok();
        return NotFound();
    }
}
