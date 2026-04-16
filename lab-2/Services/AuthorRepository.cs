using Lab2.Data;
using Lab2.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab2.Services;

public class AuthorRepository : IAuthorRepository
{
    private readonly CatalogDbContext _context;

    public AuthorRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Author>> GetAllAsync()
    {
        var authors = await _context.Authors
            .AsNoTracking()
            .OrderBy(author => author.LastName)
            .ThenBy(author => author.FirstName)
            .ToListAsync();

        return authors;
    }

    public async Task<Author?> GetByIdAsync(int id)
    {
        return await _context.Authors
            .AsNoTracking()
            .Include(author => author.Books)
                .ThenInclude(book => book.Publisher)
            .SingleOrDefaultAsync(item => item.Id == id);
    }
}
