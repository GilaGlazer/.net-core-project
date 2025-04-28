using webApiProject.Interfaces;

namespace webApiProject.Models;

public class Shoes:IIdentifiable
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int Size { get; set; }

    public string? Color { get; set; }

}
