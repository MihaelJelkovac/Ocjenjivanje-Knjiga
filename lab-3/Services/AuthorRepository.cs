using Lab3.Data;
using Lab3.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab3.Services;

public class AuthorRepository : IAuthorRepository
{
    private readonly CatalogDbContext _context;

    public AuthorRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Author>> GetAllAsync()
    {
        return await _context.Authors
            .OrderBy(author => author.LastName)
            .ThenBy(author => author.FirstName)
            .ToListAsync();
    }

    public async Task<Author?> GetByIdAsync(int id)
    {
        return await _context.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(item => item.Id == id);
    }
}
