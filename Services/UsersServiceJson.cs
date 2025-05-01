
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
            this.shoesServiceFactory = shoesServiceFactory;
            this.activeUserService = activeUserService;
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
            Console.WriteLine("Users list initialized successfully."); // Added
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
            throw; // Added: זרוק את החריגה במידת הצורך
        }
    }

    public List<Users> Get()
    {
        try // Added
        {
            Console.WriteLine("in Get");
            return usersList ?? new List<Users>();
        }
        catch (Exception ex) // Added
        {
            Console.WriteLine($"Error in Get: {ex.Message}"); // Added
            return new List<Users>(); // Added: החזר רשימה ריקה במקרה של חריגה
        }
    }
    public List<Users> GetAllItems() => null;
    public Users GetMyUser()
    {
        try // Added
        {
            Console.WriteLine("in GetMyUser");
            return usersList.FirstOrDefault(u => u.Id == activeUserService.UserId) ?? new Users(); // Updated
        }
        catch (Exception ex) // Added
        {
            Console.WriteLine($"Error in GetMyUser: {ex.Message}"); // Added
            return null; // Added: החזר null במקרה של חריגה
        }
    }

    public Users Get(int id)
    {
        // try // Added
        // {
        //     System.Console.WriteLine(usersList.FirstOrDefault(u => u.Id == id));
        //     return usersList.FirstOrDefault(u => u.Id == id);
        // }
        // catch (Exception ex) // Added
        // {
        //     Console.WriteLine($"Error in Get by ID: {ex.Message}"); // Added
            return null; // Added: החזר null במקרה של חריגה
        // }
    }

    public int Insert(Users newItem)
    {
        try // Added
        {
            Console.WriteLine("in Insert");
            if (!CheckValueRequest(newItem))
                return -1;

            int lastId = usersList.Any() ? usersList.Max(s => s.Id) : 0; // Updated
            newItem.Id = lastId + 1;
            usersList.Add(newItem);
            saveToFile();
            return newItem.Id;
        }
        catch (Exception ex) // Added
        {
            Console.WriteLine($"Error in Insert: {ex.Message}"); // Added
            return -1; // Added: החזר ערך ברירת מחדל במקרה של חריגה
        }
    }

    public bool Update(int id, Users newItem)
    {
        try // Added
        {
            Console.WriteLine("in Update");
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
            return false; // Added: החזר false במקרה של חריגה
        }
    }

    public bool Delete(int id)
    {
        System.Console.WriteLine("--------------------------in delete service------1");
        try // Added
        {
            var shoesService = shoesServiceFactory();
                   System.Console.WriteLine("--------------------------in delete service-------2");
                   System.Console.WriteLine(shoesService.Get().Count());

 // מחיקת כל הנעליים של המשתמש
            List<Shoes> userShoes = shoesService.Get().Where(shoe => shoe.UserId == id).ToList();
                    System.Console.WriteLine("--------------------------in delete service userShoes----------3");
                   System.Console.WriteLine(userShoes.Count());

            foreach (var shoe in userShoes)
            {
               // shoesService.Delete(shoe.Id); // מחיקת כל נעל של המשתמש
               Console.WriteLine($"Attempting to delete shoe with ID: {shoe.Id}");
               System.Console.WriteLine("shoe id is"+shoe.Id);
               System.Console.WriteLine("shoe user id is"+shoe.UserId);
    if (!shoesService.Delete(shoe.Id)) // בדוק אם המחיקה מצליחה
    {
        Console.WriteLine($"Failed to delete shoe with ID: {shoe.Id}");
        return false;
    }
    Console.WriteLine($"Deleted shoe with ID: {shoe.Id}");
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
            return false; // Added: החזר false במקרה של חריגה
        }
    }

    private bool CheckValueRequest(Users newItem)
    {
        try // Added
        {
            // if (newItem == null || string.IsNullOrWhiteSpace(newItem.Password)
            //     || !IsValidEmail(newItem.Email) || string.IsNullOrWhiteSpace(newItem.UserName))
            //     return false;

            // return true;
             if (newItem == null || string.IsNullOrWhiteSpace(newItem.Password)
                 || string.IsNullOrWhiteSpace(newItem.UserName))
                return false;

            return true;
        }
        catch (Exception ex) // Added
        {
            Console.WriteLine($"Error in CheckValueRequest: {ex.Message}"); // Added
            return false; // Added: החזר false במקרה של חריגה
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
            return false; // Added: החזר false במקרה של חריגה
        }
    }
}


// using Microsoft.AspNetCore.Mvc;
// using webApiProject.Models;
// using webApiProject.Interfaces;

// namespace webApiProject.Services;

// using System.Runtime.CompilerServices;
// using System.Text.Json;
// using System.Text.RegularExpressions;

// public class UsersServiceJson<T> : IService<Users> where T : class , IIdentifiable
// {
//     private readonly ActiveUserService activeUserService;
//     private readonly Func<IService<T>> shoesServiceFactory; // Factory ליצירת השירות Shoes
//     private List<Users> usersList;
//     private static string fileName = "Users.json";
//     private string filePath;

//     public UsersServiceJson(IHostEnvironment env, ActiveUserService activeUserService, Func<IService<T>> shoesServiceFactory)
//     {
//         try // Added
//         {
//             this.shoesServiceFactory = shoesServiceFactory;
//             this.activeUserService = activeUserService;
//             filePath = Path.Combine(env.ContentRootPath, "Data", fileName);

//             if (!File.Exists(filePath)) // Added
//             {
//                 Console.WriteLine($"File {filePath} does not exist. Creating a new empty list."); // Added
//                 usersList = new List<Users>();
//                 saveToFile(); // Added: יצירת קובץ ריק
//                 return;
//             }

//             using (var jsonFile = File.OpenText(filePath))
//             {
//                 usersList = JsonSerializer.Deserialize<List<Users>>(jsonFile.ReadToEnd(),
//                     new JsonSerializerOptions
//                     {
//                         PropertyNameCaseInsensitive = true
//                     }) ?? new List<Users>();
//             }
//             Console.WriteLine("Users list initialized successfully."); // Added
//         }
//         catch (Exception ex) // Added
//         {
//             Console.WriteLine($"Error initializing UsersServiceJson: {ex.Message}"); // Added
//             usersList = new List<Users>(); // Added: יצירת רשימה ריקה כברירת מחדל אם יש בעיה
//         }
//     }

//     private void saveToFile()
//     {
//         try // Added
//         {
//             File.WriteAllText(filePath, JsonSerializer.Serialize(usersList));
//         }
//         catch (Exception ex) // Added
//         {
//             Console.WriteLine($"Error saving to file: {ex.Message}"); // Added
//             throw; // Added: זרוק את החריגה במידת הצורך
//         }
//     }

//     public List<Users> Get()
//     {
//         try // Added
//         {
//             Console.WriteLine("in Get");
//             return usersList ?? new List<Users>();
//         }
//         catch (Exception ex) // Added
//         {
//             Console.WriteLine($"Error in Get: {ex.Message}"); // Added
//             return new List<Users>(); // Added: החזר רשימה ריקה במקרה של חריגה
//         }
//     }
//     public List<Users> GetAllItems() => null;
//     public Users GetMyUser()
//     {
//         try // Added
//         {
//             Console.WriteLine("in GetMyUser");
//             return usersList.FirstOrDefault(u => u.Id == activeUserService.UserId) ?? new Users(); // Updated
//         }
//         catch (Exception ex) // Added
//         {
//             Console.WriteLine($"Error in GetMyUser: {ex.Message}"); // Added
//             return null; // Added: החזר null במקרה של חריגה
//         }
//     }

//     public Users Get(int id)
//     {
//         // try // Added
//         // {
//         //     System.Console.WriteLine(usersList.FirstOrDefault(u => u.Id == id));
//         //     return usersList.FirstOrDefault(u => u.Id == id);
//         // }
//         // catch (Exception ex) // Added
//         // {
//         //     Console.WriteLine($"Error in Get by ID: {ex.Message}"); // Added
//             return null; // Added: החזר null במקרה של חריגה
//         // }
//     }

//     public int Insert(Users newItem)
//     {
//         try // Added
//         {
//             Console.WriteLine("in Insert");
//             if (!CheckValueRequest(newItem))
//                 return -1;

//             int lastId = usersList.Any() ? usersList.Max(s => s.Id) : 0; // Updated
//             newItem.Id = lastId + 1;
//             usersList.Add(newItem);
//             saveToFile();
//             return newItem.Id;
//         }
//         catch (Exception ex) // Added
//         {
//             Console.WriteLine($"Error in Insert: {ex.Message}"); // Added
//             return -1; // Added: החזר ערך ברירת מחדל במקרה של חריגה
//         }
//     }

//     public bool Update(int id, Users newItem)
//     {
//         try // Added
//         {
//             Console.WriteLine("in Update");
//             if (!CheckValueRequest(newItem) || newItem.Id != id)
//                 return false;

//             var user = usersList.FirstOrDefault(u => u.Id == id);
//             if (user == null)
//                 return false;

//             user.UserName = newItem.UserName;
//             user.Password = newItem.Password;
//             user.Email = newItem.Email;
//             user.Type = newItem.Type;
//             saveToFile();
//             return true;
//         }
//         catch (Exception ex) // Added
//         {
//             Console.WriteLine($"Error in Update: {ex.Message}"); // Added
//             return false; // Added: החזר false במקרה של חריגה
//         }
//     }

//     public bool Delete(int id)
//     {
//         System.Console.WriteLine("--------------------------in delete service------1");
//         try // Added
//         {
//             var shoesService = shoesServiceFactory();
//                    System.Console.WriteLine("--------------------------in delete service-------2");
//                    System.Console.WriteLine(shoesService.Get().Count());

//  // מחיקת כל הנעליים של המשתמש
//             List<T> userShoes = shoesService.Get().Where(shoe => shoe.UserId == id).ToList();
//                     System.Console.WriteLine("--------------------------in delete service userShoes----------3");
//                    System.Console.WriteLine(userShoes.Count());

//             foreach (var shoe in userShoes)
//             {
//                // shoesService.Delete(shoe.Id); // מחיקת כל נעל של המשתמש
//                Console.WriteLine($"Attempting to delete shoe with ID: {shoe.Id}");
//                System.Console.WriteLine("shoe id is"+shoe.Id);
//                System.Console.WriteLine("shoe user id is"+shoe.UserId);
//     if (!shoesService.Delete(shoe.Id)) // בדוק אם המחיקה מצליחה
//     {
//         Console.WriteLine($"Failed to delete shoe with ID: {shoe.Id}");
//         return false;
//     }
//     Console.WriteLine($"Deleted shoe with ID: {shoe.Id}");
//             }

//             // מחיקת המשתמש
//             var item = usersList.FirstOrDefault(s => s.Id == id);
//             if (item == null)
//                 return false;

//             usersList.Remove(item); // מחיקת המשתמש מהרשימה
//             saveToFile(); // שמירת השינויים לקובץ
//             return true;
//         }
//         catch (Exception ex) // Added
//         {
//             Console.WriteLine($"Error in Delete: {ex.Message}"); // Added
//             return false; // Added: החזר false במקרה של חריגה
//         }
//     }

//     private bool CheckValueRequest(Users newItem)
//     {
//         try // Added
//         {
//             // if (newItem == null || string.IsNullOrWhiteSpace(newItem.Password)
//             //     || !IsValidEmail(newItem.Email) || string.IsNullOrWhiteSpace(newItem.UserName))
//             //     return false;

//             // return true;
//              if (newItem == null || string.IsNullOrWhiteSpace(newItem.Password)
//                  || string.IsNullOrWhiteSpace(newItem.UserName))
//                 return false;

//             return true;
//         }
//         catch (Exception ex) // Added
//         {
//             Console.WriteLine($"Error in CheckValueRequest: {ex.Message}"); // Added
//             return false; // Added: החזר false במקרה של חריגה
//         }
//     }

//     private bool IsValidEmail(string email)
//     {
//         try // Added
//         {
//             if (string.IsNullOrEmpty(email))
//                 return false;

//             // ביטוי רגולרי לבדוק אם המייל תקין
//             string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
//             return Regex.IsMatch(email, pattern);
//         }
//         catch (Exception ex) // Added
//         {
//             Console.WriteLine($"Error in IsValidEmail: {ex.Message}"); // Added
//             return false; // Added: החזר false במקרה של חריגה
//         }
//     }
// }