using Microsoft.EntityFrameworkCore;
using project.Models;

namespace project.Data;

public class UserDbRepository : IUserDbRepository
{
    private readonly AppDbContext _context;

    public UserDbRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetByGuid(Guid guid)
    {
        return await _context.Users.FirstOrDefaultAsync(u=>u.Guid == guid);
    }


    public bool Add(User user)
    {
        _context.Users.Add(user);
        return Save();
    }

    public bool Update(User user)
    {
        _context.Users.Update(user);
        return Save();
    }

    public bool Remove(User user)
    {
        _context.Users.Remove(user);
        return Save();
    }

    public bool Save()
    {
        var saved = _context.SaveChanges();
        return saved > 0;
    }
}