using webApiProject.Models;
namespace webApiProject.Interfaces;

public interface IUsersService
{
     List<Users> Get();
     Users Get(string password);
     int Insert(Users newItem);
     bool Update(string password, Users newItem);
     bool Delete(string password);
}
