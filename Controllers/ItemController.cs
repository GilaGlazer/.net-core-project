// using Microsoft.AspNetCore.Mvc;
// using webApiProject.Models;
// using webApiProject.Interfaces;

// namespace webApiProject.Controllers;

// [ApiController]
// [Route("[controller]")]
// public class ItemController<T> : ControllerBase where T : class
// {
//     private IService<T> itemService;

//     public ItemController(IService<T> itemService)
//     {
//         this.itemService = itemService;
//     }
    
//     [HttpGet]
//     public ActionResult<IEnumerable<T>> Get()
//     {
//         return itemService.Get();
//     }

//     [HttpGet("{id}")]
//     public ActionResult<T> Get(int id)
//     {
//         var item = itemService.Get(id);
//         if (item == null)
//             return NotFound();
//         return item;
//     }

//     [HttpPost]
//     public ActionResult Post(T newItem)
//     {
//         var newId = itemService.Insert(newItem);
//         if (newId == -1)
//             return BadRequest();
//         return CreatedAtAction(nameof(Post), new { Id = newId });
//     }

//     [HttpPut("{id}")]
//     public ActionResult Put(int id, T newItem)
//     {
//         if (itemService.Update(id, newItem))
//             return NoContent();
//         return BadRequest();
//     }

//     [HttpDelete("{id}")]
//     public ActionResult Delete(int id)
//     {
//         if (itemService.Delete(id))
//             return Ok();
//         return NotFound();
//     }
// }
