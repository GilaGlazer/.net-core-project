using Microsoft.AspNetCore.Mvc;
using webApiProject.Models;
using webApiProject.Interfaces;
namespace webApiProject.Services;

using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;

public class UsersServiceJson : IUsersService
{

    private List<Users> usersList;
    private static string fileName = "Users.json";
    private string filePath;
    public UsersServiceJson(IHostEnvironment env)
    {
        filePath = Path.Combine(env.ContentRootPath, "Data", fileName);

        using (var jsonFile = File.OpenText(filePath))
        {
            usersList = JsonSerializer.Deserialize<List<Users>>(jsonFile.ReadToEnd(),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<Users>();
        }
    }
    private void saveToFile()
    {
        File.WriteAllText(filePath, JsonSerializer.Serialize(usersList));
    }

    public List<Users> Get() => usersList;
    public Users Get(string email) => usersList.FirstOrDefault(u => u.Email.Equals(email));
    public int Insert(Users newItem)
    {
        if (!CheckValueRequest(newItem))
            return -1;

        int lastId = usersList.Max(s => s.Id);
        newItem.Id = lastId + 1;
        usersList.Add(newItem);
        saveToFile();
        return newItem.Id;
    }

    public bool Update(string email, Users newItem)
    {
        if (!CheckValueRequest(newItem) || !newItem.Email.Equals(email))
            return false;
        var user = usersList.FirstOrDefault(u => u.Email.Equals(email));
        if (user == null)
            return false;
        user.UserName = newItem.UserName;
        user.Password = newItem.Password;
        user.Email = newItem.Email;
        user.Type = newItem.Type;
        saveToFile();
        return true;
    }

    public bool Delete(string email)
    {
        var user = usersList.FirstOrDefault(u => u.Email.Equals(email));
        if (user == null)
            return false;
        var index = usersList.IndexOf(user);
        usersList.RemoveAt(index);
        saveToFile();
        return true;
    }

    private bool CheckValueRequest(Users newItem)
    {
        if (newItem == null || string.IsNullOrWhiteSpace(newItem.Password)
        || !IsValidEmail(newItem.Email) || string.IsNullOrWhiteSpace(newItem.UserName))
            return false;
        return true;
    }
    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;

        // ביטוי רגולרי לבדוק אם המייל תקין
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

}


public static class UsersUtilitiesJson
{
    public static void AddUsersJson(this IServiceCollection services)
    {
        services.AddSingleton<IUsersService, UsersServiceJson>();
    }
}
