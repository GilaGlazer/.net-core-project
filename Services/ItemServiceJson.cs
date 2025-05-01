using Microsoft.AspNetCore.Mvc;
using webApiProject.Models;
using webApiProject.Interfaces;
namespace webApiProject.Services;
using System.Text.Json;
using System.Text.RegularExpressions;

public class ItemServiceJson<T> : IService<T> where T : class, IIdentifiable, new()
{
    private readonly ActiveUserService activeUserService;
    private readonly Func<IService<Users>> usersServiceFactory; // Factory ליצירת השירות Users

    private List<T> itemList;
    private static string fileName = typeof(T).Name + ".json";
    private string filePath;
    public ItemServiceJson(IHostEnvironment env, ActiveUserService activeUserService,  Func<IService<Users>> usersServiceFactory)
    {
        try // Added
        {
            this.activeUserService = activeUserService;
            this.usersServiceFactory = usersServiceFactory;
            filePath = Path.Combine(env.ContentRootPath, "Data", fileName);
if (!File.Exists(filePath)) // Added
            {
                Console.WriteLine($"File {filePath} does not exist. Creating a new empty list."); // Added
                itemList = new List<T>();
                saveToFile(); // Added: יצירת קובץ ריק
                return;
            }
            using (var jsonFile = File.OpenText(filePath))
            {
                itemList = JsonSerializer.Deserialize<List<T>>(jsonFile.ReadToEnd(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<T>();
            }
        }
        catch (Exception ex) // Added
        {
            Console.WriteLine($"Error initializing ItemServiceJson: {ex.Message}"); // Added
            itemList = new List<T>(); // יצירת רשימה ריקה כברירת מחדל אם יש בעיה
        }
    }
    private void saveToFile()
    {
        try // Added
        {
            File.WriteAllText(filePath, JsonSerializer.Serialize(itemList));
        }
        catch (Exception ex) // Added
        {
            Console.WriteLine($"Error saving to file: {ex.Message}"); // Added
            throw; // ניתן לזרוק מחדש את החריגה אם חשוב להפסיק את הביצוע
        }
    }

    public List<T> Get()
    {
        System.Console.WriteLine("Get shoes");
        try // Added
        {
             if(activeUserService.Type == "admin")
            {
                System.Console.WriteLine("admin want to get item");
            return itemList;
            }
            return itemList.Where(i => i.UserId == activeUserService.UserId).ToList();
        }
        catch (Exception ex) // Added
        {
            Console.WriteLine($"Error in Get: {ex.Message}"); // Added
            return new List<T>(); // חזרה לרשימה ריקה במקרה של חריגה
        }
    }
    public T Get(int id)
    {
        
        try // Added
        {
            if(activeUserService.Type == "admin")
            {
                System.Console.WriteLine("admin want to get item");
            return itemList.FirstOrDefault(i => i.Id == id );
            }
            return itemList.FirstOrDefault(i => i.Id == id && i.UserId == activeUserService.UserId);
        }
        catch (Exception ex) // Added
        {
            Console.WriteLine($"Error in Get by ID: {ex.Message}"); // Added
            return null; // חזרה לערך ברירת מחדל במקרה של חריגה
        }
    }
    public int Insert(T newItem)
    {
        try // Added
        {
            if (!CheckValueRequest(newItem))
                return -1;
            if (activeUserService.Type == "admin")
            {       
                if (string.IsNullOrEmpty(newItem.UserId.ToString()))
                    return -1;
                var usersService = usersServiceFactory();
                var user = usersService.Get(newItem.UserId);
                if (user == null)
                    return -1;
            }
            else newItem.UserId = activeUserService.UserId;
            //    Console.WriteLine("Insert item: " + newItem.ToString());
            int lastId = itemList.Any() ? itemList.Max(s => s.Id) : 0; // Updated to handle empty list
            newItem.Id = lastId + 1;
            itemList.Add(newItem);
            saveToFile();
            return newItem.Id;
        }
        catch (Exception ex) // Added
        {
            Console.WriteLine($"Error in Insert: {ex.Message}"); // Added
            return -1; // חזרה לערך ברירת מחדל במקרה של חריגה
        }
    }

    public bool Update(int id, T newItem)
    {
        try // Added
        {
            if (!CheckValueRequest(newItem) || newItem.Id != id)
                return false;
            var item = itemList.FirstOrDefault(s => s.Id == id);
            if (item == null || item.UserId != activeUserService.UserId)
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
        catch (Exception ex) // Added
        {
            Console.WriteLine($"Error in Update: {ex.Message}"); // Added
            return false; // חזרה לערך ברירת מחדל במקרה של חריגה
        }
    }
public bool Delete(int id)
{
    System.Console.WriteLine("in delete item service begin");
    try
    {
        var item = itemList.FirstOrDefault(s => s.Id == id);
                // בדיקה אם המשתמש הוא בעל הפריט או מנהל
        if (item == null || (activeUserService.Type != "admin" && item.UserId != activeUserService.UserId))
            return false;

        itemList.Remove(item);
        System.Console.WriteLine("in delete item service before save");
        saveToFile();
        System.Console.WriteLine("in delete item service after save");
        System.Console.WriteLine("item deleted");
        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in Delete: {ex.Message}");
        return false;
    }
}



    private bool CheckValueRequest(T newItem)
    {
        try // Added
        {
            if (newItem == null)
                return false;

            var propertiesToCheck = new Dictionary<string, Func<object, bool>>
            {
                { "Name", value => !string.IsNullOrWhiteSpace(value?.ToString()) },
                { "Color", value => !string.IsNullOrWhiteSpace(value?.ToString()) },
                { "Size", value => value is int size && size >= 18 && size < 50 },
                { "Email", value => !string.IsNullOrWhiteSpace(value?.ToString()) && IsValidEmail(value.ToString()) },
                { "Password", value => !string.IsNullOrWhiteSpace(value?.ToString()) }
            };

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
        catch (Exception ex) // Added
        {
            Console.WriteLine($"Error in CheckValueRequest: {ex.Message}"); // Added
            return false; // חזרה לערך ברירת מחדל במקרה של חריגה
        }
    }

    private bool IsValidEmail(string email)
    {
        try // Added
        {
            if (string.IsNullOrEmpty(email))
                return false;

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }
        catch (Exception ex) // Added
        {
            Console.WriteLine($"Error in IsValidEmail: {ex.Message}"); // Added
            return false; // חזרה לערך ברירת מחדל במקרה של חריגה
        }
    }
        public List<T> GetAllItems()
    {
        System.Console.WriteLine("Get all shoes");
        try // Added
        {
            return itemList; // החזרת כל הרשימה
        }
        catch (Exception ex) // Added
        {
            Console.WriteLine($"Error in Get: {ex.Message}"); // Added
            return new List<T>(); // חזרה לרשימה ריקה במקרה של חריגה
        }
    }

}