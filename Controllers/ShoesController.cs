using Microsoft.AspNetCore.Mvc;
using webApiProject.Models;
using webApiProject.Services;
namespace webApiProject.Controllers;

[ApiController]
[Route("[controller]")]
public class ShoesController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Shoes>> Get()
    {
        return ShoesService.Get();
    }

    [HttpGet("{id}")]
    public ActionResult<Shoes> Get(int id)
    {
        var shoe = ShoesService.Get(id);
        if (shoe == null)
            return NotFound();
        return shoe;
    }

    [HttpPost]
    public ActionResult Post(Shoes newItem)
    {
        var newId = ShoesService.Insert(newItem);
        if (newId == -1)
            return BadRequest();
        return CreatedAtAction(nameof(Post), new { Id = newId });
    }

    [HttpPut("{id}")]
    public ActionResult Put(int id, Shoes newItem)
    {
        if (ShoesService.Update(id, newItem))
            return NoContent();
        return BadRequest();
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        if (ShoesService.Delete(id))
            return Ok();
        return NotFound();
    }
}
