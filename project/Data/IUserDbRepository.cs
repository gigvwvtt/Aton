using project.Models;

namespace project.Data;

public interface IUserDbRepository
{
    Task<IEnumerable<User>?> GetAll();
    Task<IEnumerable<User>?> GetAllActive();
    Task<IEnumerable<User>?> GetAllOlderThan(int age);
    Task<User?> GetByLogin(string login);
    Task<User?> GetByGuid(Guid guid);
    bool IsUserExist(string login);
    bool IsUserExist(Guid guid);
    bool IsUserDeleted(string login);
    bool IsUserAdmin(string login);
    bool Add(User user);
    bool Update(User user);
    bool Remove(User user);
    bool Save();
    
}