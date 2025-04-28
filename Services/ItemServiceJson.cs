using Microsoft.AspNetCore.Mvc;
using webApiProject.Models;
using webApiProject.Interfaces;
namespace webApiProject.Services;
using System.Text.Json;
using System.Text.RegularExpressions;

public class ItemServiceJson<T> : IService<T> where T : class, IIdentifiable, new()
{

    private List<T> itemList;
    private static string fileName = typeof(T).Name + ".json";
    private string filePath;
    public ItemServiceJson(IHostEnvironment env)
    {
        filePath = Path.Combine(env.ContentRootPath, "Data", fileName);

        using (var jsonFile = File.OpenText(filePath))
        {
            itemList = JsonSerializer.Deserialize<List<T>>(jsonFile.ReadToEnd(),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<T>();
        }
    }
    private void saveToFile()
    {
        File.WriteAllText(filePath, JsonSerializer.Serialize(itemList));
    }

    public List<T> Get() => itemList;

    public T Get(int id) => itemList.FirstOrDefault(s => s.Id == id);
    public int Insert(T newItem)
    {
    //    Console.WriteLine("Insert item: " + newItem.ToString());
        if (!CheckValueRequest(newItem))
            return -1;
        int lastId = itemList.Max(s => s.Id);
        newItem.Id = lastId + 1;
        itemList.Add(newItem);
        saveToFile();
        return newItem.Id;
    }

    public bool Update(int id, T newItem)
    {
        if (!CheckValueRequest(newItem) || newItem.Id != id)
            return false;
        var item = itemList.FirstOrDefault(s => s.Id == id);
        if (item == null)
            return false;
        foreach (var property in typeof(T).GetProperties())
        {
            if (property.CanWrite)
            {
                var newValue = property.GetValue(newItem);
                property.SetValue(item, newValue);
            }
        }
        saveToFile();
        return true;
    }

    public bool Delete(int id)
    {
        var item = itemList.FirstOrDefault(s => s.Id == id);
        if (item == null)
            return false;
        var index = itemList.IndexOf(item);
        itemList.RemoveAt(index);
        saveToFile();
        return true;
    }


    private bool CheckValueRequest(T newItem)
    {
        if (newItem == null)
            return false;

        var propertiesToCheck = new Dictionary<string, Func<object, bool>>
    {
        { "Name", value => !string.IsNullOrWhiteSpace(value?.ToString()) },
        { "Color", value => !string.IsNullOrWhiteSpace(value?.ToString()) },
        { "Size", value => value is int size && size >= 18 },
        { "Email", value => !string.IsNullOrWhiteSpace(value?.ToString()) && IsValidEmail(value.ToString()) },
        { "Password", value => !string.IsNullOrWhiteSpace(value?.ToString()) }
    };

        // בדיקת כל תכונה ברשימה
        foreach (var propertyCheck in propertiesToCheck)
        {
            var property = typeof(T).GetProperty(propertyCheck.Key);
            if (property != null)
            {
                var value = property.GetValue(newItem);
                if (!propertyCheck.Value(value))
                    return false;
            }
        }

        return true;
    }

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;

        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

}

public static class ItemUtilitiesJson
{
    public static void AddItemJson<T>(this IServiceCollection services) where T : class, IIdentifiable, new()
    {
        services.AddSingleton<IService<T>, ItemServiceJson<T>>();
    }
}