using Microsoft.AspNetCore.Mvc;
using webApiProject.Models;
using webApiProject.Interfaces;
using Microsoft.AspNetCore.Authorization;
using webApiProject.Services;
using System.Security.Claims;

namespace webApiProject.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Policy = "user")]

public class ShoesController : ControllerBase
{
    private readonly IService<Shoes> shoesService;

    public ShoesController(IService<Shoes> shoesService)
    {
        this.shoesService = shoesService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Shoes>> Get()
    {
        System.Console.WriteLine("Get shoes");
        return shoesService.Get();
    }

    [HttpGet("{id}")]
    public ActionResult<Shoes> Get(int id)
    {
        System.Console.WriteLine("Get shoes with id");
        var shoe = shoesService.Get(id);
        if (shoe == null)
            return NotFound();
        return shoe;
    }

    [HttpPost]
    public ActionResult Post(Shoes newShoe)
    {
        var newId = shoesService.Insert(newShoe);
        if (newId == -1)
            return BadRequest();
        return CreatedAtAction(nameof(Post), new { Id = newId });
    }

    [HttpPut("{id}")]
    public ActionResult Put(int id, Shoes updatedShoe)
    {
        if (shoesService.Update(id, updatedShoe))
            return NoContent();
        return BadRequest();
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        if (shoesService.Delete(id))
            return Ok();
        return NotFound();
    }
}