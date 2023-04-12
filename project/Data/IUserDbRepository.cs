using project.Models;

namespace project.Data;

public interface IUserDbRepository
{
    Task<IEnumerable<User>> GetAll();
    Task<User?> GetByGuid(Guid guid);
    bool Add(User user);
    bool Update(User user);
    bool Remove(User user);
    bool Save();
    
}