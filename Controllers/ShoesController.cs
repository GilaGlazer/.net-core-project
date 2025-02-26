using Microsoft.AspNetCore.Mvc;
using webApiProject.Models;
using webApiProject.Interfaces;

namespace webApiProject.Controllers;

[ApiController]
[Route("[controller]")]
public class ShoesController : ControllerBase
{
    private IShoesService shoesService;

    public ShoesController(IShoesService shoesService)
    {
        this.shoesService = shoesService;
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<Shoes>> Get()
    {
        return shoesService.Get();
    }

    [HttpGet("{id}")]
    public ActionResult<Shoes> Get(int id)
    {
        var shoe = shoesService.Get(id);
        if (shoe == null)
            return NotFound();
        return shoe;
    }

    [HttpPost]
    public ActionResult Post(Shoes newItem)
    {
        var newId = shoesService.Insert(newItem);
        if (newId == -1)
            return BadRequest();
        return CreatedAtAction(nameof(Post), new { Id = newId });
    }

    [HttpPut("{id}")]
    public ActionResult Put(int id, Shoes newItem)
    {
        if (shoesService.Update(id, newItem))
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
