using Microsoft.AspNetCore.Mvc;
using webApiProject.Models;
namespace webApiProject.Services;

public static class ShoesService
{

    private static List<Shoes> shoesList;

    static ShoesService()
    {
        shoesList = new List<Shoes>
        {
            new Shoes { Id = 1 ,Name= "high heels", Size = 37, Color = "red"},
            new Shoes { Id = 2 , Name= "elegant shoes", Size = 38, Color = "black"},
            new Shoes { Id = 3 , Name= "boots", Size = 39, Color = "yellow"},
            new Shoes { Id = 4 , Name= "Sports shoes", Size = 40, Color = "green"}
        };
    }

    public static List<Shoes> Get()
    {
        return shoesList;
    }

    public static Shoes Get(int id)
    {
        return shoesList.FirstOrDefault(s => s.Id == id);
    }
    public static int Insert(Shoes newItem)
    {
        if (!CheckValueRequest(newItem))
            return -1;

        int lastId = shoesList.Max(s => s.Id);
        newItem.Id = lastId + 1;
        shoesList.Add(newItem);
        return newItem.Id;
    }

    public static bool Update(int id, Shoes newItem)
    {
        if (!CheckValueRequest(newItem) || newItem.Id != id)
            return false;
        var shoe = shoesList.FirstOrDefault(s => s.Id == id);
        if (shoe == null)
            return false;
        shoe.Name = newItem.Name;
        shoe.Size = newItem.Size;
        shoe.Color = newItem.Color;
        return true;
    }

    public static bool Delete(int id)
    {
        var shoe = shoesList.FirstOrDefault(s => s.Id == id);
        if (shoe == null)
            return false;
        var index = shoesList.IndexOf(shoe);
        shoesList.RemoveAt(index);
        return true;
    }

    private static bool CheckValueRequest(Shoes newItem)
    {
        if (newItem == null || string.IsNullOrWhiteSpace(newItem.Name)
        || string.IsNullOrWhiteSpace(newItem.Color) || newItem.Size < 18)
            return false;
        return true;
    }

}
