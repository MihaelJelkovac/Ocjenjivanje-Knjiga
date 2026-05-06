using Lab3.Data;
using Lab3.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab3.Services;

public class UserRepository : IUserRepository
{
    private readonly CatalogDbContext _context;

    public UserRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<User>> GetAllAsync()
    {
        return await _context.Users
            .Include(u => u.Reviews)
            .OrderBy(user => user.FullName)
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Reviews)
            .FirstOrDefaultAsync(item => item.Id == id);
    }
}
