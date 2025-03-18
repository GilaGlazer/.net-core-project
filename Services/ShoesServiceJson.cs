using Microsoft.AspNetCore.Mvc;
using webApiProject.Models;
using webApiProject.Interfaces;
namespace webApiProject.Services;
using System.Text.Json;

public class ShoesServiceJson : IShoesService
{

    private List<Shoes> shoesList;
    private static string fileName = "Shoes.json";
    private string filePath;
    public ShoesServiceJson(IHostEnvironment env)
    {
        filePath = Path.Combine(env.ContentRootPath, "Data", fileName);

        using (var jsonFile = File.OpenText(filePath))
        {
            shoesList = JsonSerializer.Deserialize<List<Shoes>>(jsonFile.ReadToEnd(),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<Shoes>();
        }
    }
    private void saveToFile()
    {
        File.WriteAllText(filePath, JsonSerializer.Serialize(shoesList));
    }

    public List<Shoes> Get() => shoesList;

    public Shoes Get(int id) => shoesList.FirstOrDefault(s => s.Id == id);
    public int Insert(Shoes newItem)
    {
        if (!CheckValueRequest(newItem))
            return -1;

        int lastId = shoesList.Max(s => s.Id);
        newItem.Id = lastId + 1;
        shoesList.Add(newItem);
        saveToFile();
        return newItem.Id;
    }

    public bool Update(int id, Shoes newItem)
    {
        if (!CheckValueRequest(newItem) || newItem.Id != id)
            return false;
        var shoe = shoesList.FirstOrDefault(s => s.Id == id);
        if (shoe == null)
            return false;
        shoe.Name = newItem.Name;
        shoe.Size = newItem.Size;
        shoe.Color = newItem.Color;
        saveToFile();
        return true;
    }

    public bool Delete(int id)
    {
        var shoe = shoesList.FirstOrDefault(s => s.Id == id);
        if (shoe == null)
            return false;
        var index = shoesList.IndexOf(shoe);
        shoesList.RemoveAt(index);
        saveToFile();
        return true;
    }

    private bool CheckValueRequest(Shoes newItem)
    {
        if (newItem == null || string.IsNullOrWhiteSpace(newItem.Name)
        || string.IsNullOrWhiteSpace(newItem.Color) || newItem.Size < 18)
            return false;
        return true;
    }

}


public static class ShoesUtilitiesJson
{
    public static void AddShoesJson(this IServiceCollection services)
    {
        services.AddSingleton<IShoesService, ShoesServiceJson>();
    }
}
