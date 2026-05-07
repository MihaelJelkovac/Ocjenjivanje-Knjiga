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
        // Include Books to avoid N+1 when Views access author.Books.Count
        return await _context.Authors
            .Include(a => a.Books)
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
