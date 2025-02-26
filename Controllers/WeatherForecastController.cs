using Microsoft.AspNetCore.Mvc;
using webApiProject.Models;
namespace webApiProject.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly List<WeatherForecast> listDays;

    public WeatherForecastController()
    {
        listDays = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToList(); ;

    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return listDays;
    }

    [HttpGet("{id}")]
    public ActionResult<WeatherForecast> Get(int id)
    {
        if (id < 0 || id > listDays.Count())
            return BadRequest("Not Valid");
        return listDays[id];
    }

    [HttpPost]
    public void Post(WeatherForecast newItem)
    {
        listDays.Add(newItem);
    }

    [HttpPut("{id}")]
    public ActionResult Put(int id, WeatherForecast newItem)
    {
        if (id < 0 || id > listDays.Count())
            return BadRequest("Not Valid");
        listDays[id] = newItem;
        return NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult RuthDelete(int id)
    {
        if (id < 0 || id > listDays.Count())
            return BadRequest("Not Valid"); ;
        listDays.RemoveAt(id);
        return NoContent();
    }


}
