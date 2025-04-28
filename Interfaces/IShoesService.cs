using webApiProject.Models;
namespace webApiProject.Interfaces;

public interface IService
{

     List<Shoes> Get();
     Shoes Get(int id);
     int Insert(Shoes newItem);
     bool Update(int id, Shoes newItem);
     bool Delete(int id);
}
