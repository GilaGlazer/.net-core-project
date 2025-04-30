// using Microsoft.AspNetCore.Mvc;
// using webApiProject.Models;
// using webApiProject.Interfaces;
// namespace webApiProject.Services;

// using System.Runtime.CompilerServices;
// using System.Text.Json;
// using System.Text.RegularExpressions;

// public class UsersServiceJson : IService<Users>
// {
//     private readonly ActiveUserService activeUserService;
//     private readonly Func<IService<Shoes>> shoesServiceFactory; // Factory ליצירת השירות Shoes
//     private List<Users> usersList;
//     private static string fileName = "Users.json";
//     private string filePath;
//     public UsersServiceJson(IHostEnvironment env, ActiveUserService activeUserService,  Func<IService<Shoes>> shoesServiceFactory)
//     {
//         this.shoesServiceFactory = shoesServiceFactory;
//         this.activeUserService = activeUserService;
//         filePath = Path.Combine(env.ContentRootPath, "Data", fileName);

//         using (var jsonFile = File.OpenText(filePath))
//         {
//             usersList = JsonSerializer.Deserialize<List<Users>>(jsonFile.ReadToEnd(),
//             new JsonSerializerOptions
//             {
//                 PropertyNameCaseInsensitive = true
//             }) ?? new List<Users>();
//         }
//     }
//     private void saveToFile()
//     {
//         File.WriteAllText(filePath, JsonSerializer.Serialize(usersList));
//     }

//     // public List<Users> Get() => usersList ?? new List<Users>();
//     public List<Users> Get()
//     {
//         System.Console.WriteLine("in Get");
//         return usersList ?? new List<Users>();
//     }



//     public Users GetMyUser()
//     {
//         System.Console.WriteLine("in GetMyUser");
//         return usersList.Where(u => u.Id == activeUserService.UserId).FirstOrDefault();
//     }

//     public Users Get(int id) => usersList.FirstOrDefault(s => s.Id == id);
//     public int Insert(Users newItem)
//     {
//         System.Console.WriteLine("in Insert");
//         if (!CheckValueRequest(newItem))
//             return -1;
//         int lastId = usersList.Max(s => s.Id);
//         newItem.Id = lastId + 1;
//         usersList.Add(newItem);
//         saveToFile();
//         return newItem.Id;
//     }

//     public bool Update(int id, Users newItem)
//     {
//         System.Console.WriteLine("in Update");
//         if (!CheckValueRequest(newItem) || newItem.Id != id)
//             return false;
//         var user = usersList.FirstOrDefault(u => u.Id == id);
//         if (user == null)
//             return false;
//         user.UserName = newItem.UserName;
//         user.Password = newItem.Password;
//         user.Email = newItem.Email;
//         user.Type = newItem.Type;
//         saveToFile();
//         return true;
//     }

//     public bool Delete(int id)
//     {
//         var shoesService = shoesServiceFactory();

//         // מחיקת כל הנעליים של המשתמש
//         var userShoes = shoesService.Get().Where(shoe => shoe.UserId == id).ToList();
//         foreach (var shoe in userShoes)
//         {
//             shoesService.Delete(shoe.Id); // מחיקת כל נעל של המשתמש
//         }

//         // מחיקת המשתמש
//         var item = usersList.FirstOrDefault(s => s.Id == id);
//         if (item == null)
//             return false;
//         usersList.Remove(item); // מחיקת המשתמש מהרשימה
//         saveToFile(); // שמירת השינויים לקובץ
//         return true;
//     }

//     private bool CheckValueRequest(Users newItem)
//     {
//         if (newItem == null || string.IsNullOrWhiteSpace(newItem.Password)
//         || !IsValidEmail(newItem.Email) || string.IsNullOrWhiteSpace(newItem.UserName))
//             return false;
//         return true;
//     }
//     private bool IsValidEmail(string email)
//     {
//         if (string.IsNullOrEmpty(email))
//             return false;

//         // ביטוי רגולרי לבדוק אם המייל תקין
//         string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
//         return Regex.IsMatch(email, pattern);
//     }

// }


// public static class UsersUtilitiesJson
// {
//     public static void AddUsersJson(this IServiceCollection services)
//     {
//         services.AddScoped<IService<Users>, UsersServiceJson>();
//     }
// }
using Microsoft.AspNetCore.Mvc;
using webApiProject.Models;
using webApiProject.Interfaces;

namespace webApiProject.Services;

using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;

public class UsersServiceJson : IService<Users> 
{
    private readonly ActiveUserService activeUserService;
    private readonly Func<IService<Shoes>> shoesServiceFactory; // Factory ליצירת השירות Shoes
    private List<Users> usersList;
    private static string fileName = "Users.json";
    private string filePath;

    public UsersServiceJson(IHostEnvironment env, ActiveUserService activeUserService, Func<IService<Shoes>> shoesServiceFactory)
    {
        try // Added
        {
            this.activeUserService = activeUserService;
                        this.shoesServiceFactory = shoesServiceFactory;

            filePath = Path.Combine(env.ContentRootPath, "Data", fileName);

            if (!File.Exists(filePath)) // Added
            {
                Console.WriteLine($"File {filePath} does not exist. Creating a new empty list."); // Added
                usersList = new List<Users>();
                saveToFile(); // Added: יצירת קובץ ריק
                return;
            }

            using (var jsonFile = File.OpenText(filePath))
            {
                usersList = JsonSerializer.Deserialize<List<Users>>(jsonFile.ReadToEnd(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<Users>();
            }
            System.Console.WriteLine("Users list initialized successfully."); // Added
        }
        catch (Exception ex) // Added
        {
            Console.WriteLine($"Error initializing UsersServiceJson: {ex.Message}"); // Added
            usersList = new List<Users>(); // Added: יצירת רשימה ריקה כברירת מחדל אם יש בעיה
        }
    }

    private void saveToFile()
    {
        try // Added
        {
            File.WriteAllText(filePath, JsonSerializer.Serialize(usersList));
        }
        catch (Exception ex) // Added
        {
            Console.WriteLine($"Error saving to file: {ex.Message}"); // Added
            throw; // ניתן לזרוק מחדש את החריגה אם חשוב להפסיק את הביצוע
        }
    }

    public List<Users> Get()
    {
        try // Added
        {
            System.Console.WriteLine("in Get");
            return usersList.ToList(); // Updated: חזרה לרשימה ריקה במקרה של null
        }
        catch (Exception ex) // Added
        {
            Console.WriteLine($"Error in Get: {ex.Message}"); // Added
            return new List<Users>(); // Added: חזרה לרשימה ריקה במקרה של חריגה
        }
    }

    public Users GetMyUser()
    {
        try // Added
        {
            System.Console.WriteLine("in GetMyUser");
            return usersList.FirstOrDefault(u => u.Id == activeUserService.UserId) ?? new Users(); // Updated: חזרה לערך ברירת מחדל אם לא נמצא משתמש
        }
        catch (Exception ex) // Added
        {
            Console.WriteLine($"Error in GetMyUser: {ex.Message}"); // Added
            return null; // Added: חזרה לערך ברירת מחדל במקרה של חריגה
        }
    }

    public Users Get(int id)
    {
        try // Added
        {
            return usersList.FirstOrDefault(s => s.Id == id);
        }
        catch (Exception ex) // Added
        {
            Console.WriteLine($"Error in Get by ID: {ex.Message}"); // Added
            return null; // Added: חזרה לערך ברירת מחדל במקרה של חריגה
        }
    }

    public int Insert(Users newItem)
    {
        try // Added
        {
            System.Console.WriteLine("in Insert");
            if (!CheckValueRequest(newItem))
                return -1;

            int lastId = usersList.Any() ? usersList.Max(s => s.Id) : 0; // Updated: שימוש ב-0 אם הרשימה ריקה
            newItem.Id = lastId + 1;
            usersList.Add(newItem);
            saveToFile();
            return newItem.Id;
        }
        catch (Exception ex) // Added
        {
            Console.WriteLine($"Error in Insert: {ex.Message}"); // Added
            return -1; // Added: חזרה לערך ברירת מחדל במקרה של חריגה
        }
    }

    public bool Update(int id, Users newItem)
    {
        try // Added
        {
            System.Console.WriteLine("in Update");
            if (!CheckValueRequest(newItem) || newItem.Id != id)
                return false;

            var user = usersList.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return false;

            user.UserName = newItem.UserName;
            user.Password = newItem.Password;
            user.Email = newItem.Email;
            user.Type = newItem.Type;
            saveToFile();
            return true;
        }
        catch (Exception ex) // Added
        {
            Console.WriteLine($"Error in Update: {ex.Message}"); // Added
            return false; // Added: חזרה לערך ברירת מחדל במקרה של חריגה
        }
    }

    public bool Delete(int id)
    {
        try // Added
        {
            var shoesService = shoesServiceFactory();

            // מחיקת כל הנעליים של המשתמש
            var userShoes = shoesService.Get().Where(shoe => shoe.UserId == id).ToList();
            foreach (var shoe in userShoes)
            {
                shoesService.Delete(shoe.Id); // מחיקת כל נעל של המשתמש
            }

            // מחיקת המשתמש
            var item = usersList.FirstOrDefault(s => s.Id == id);
            if (item == null)
                return false;

            usersList.Remove(item); // מחיקת המשתמש מהרשימה
            saveToFile(); // שמירת השינויים לקובץ
            return true;
        }
        catch (Exception ex) // Added
        {
            Console.WriteLine($"Error in Delete: {ex.Message}"); // Added
            return false; // Added: חזרה לערך ברירת מחדל במקרה של חריגה
        }
    }

    private bool CheckValueRequest(Users newItem)
    {
        try // Added
        {
            if (newItem == null || string.IsNullOrWhiteSpace(newItem.Password)
            || !IsValidEmail(newItem.Email) || string.IsNullOrWhiteSpace(newItem.UserName))
                return false;

            return true;
        }
        catch (Exception ex) // Added
        {
            Console.WriteLine($"Error in CheckValueRequest: {ex.Message}"); // Added
            return false; // Added: חזרה לערך ברירת מחדל במקרה של חריגה
        }
    }

    private bool IsValidEmail(string email)
    {
        try // Added
        {
            if (string.IsNullOrEmpty(email))
                return false;

            // ביטוי רגולרי לבדוק אם המייל תקין
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }
        catch (Exception ex) // Added
        {
            Console.WriteLine($"Error in IsValidEmail: {ex.Message}"); // Added
            return false; // Added: חזרה לערך ברירת מחדל במקרה של חריגה
        }
    }
}

// public static class UsersUtilitiesJson
// {
//     public static void AddUsersJson(this IServiceCollection services)
//     {
//         services.AddScoped<IService<Users>, UsersServiceJson>();
//     }
// }