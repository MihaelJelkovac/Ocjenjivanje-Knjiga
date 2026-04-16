using Lab2.Data;
using Lab2.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab2.Services;

public class UserRepository : IUserRepository
{
    private readonly CatalogDbContext _context;

    public UserRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<User>> GetAllAsync()
    {
        var users = await _context.Users
            .AsNoTracking()
            .OrderByDescending(user => user.ReputationPoints)
            .ThenBy(user => user.FullName)
            .ToListAsync();

        return users;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .AsNoTracking()
            .Include(user => user.Reviews)
                .ThenInclude(review => review.Book)
            .SingleOrDefaultAsync(item => item.Id == id);
    }
}
