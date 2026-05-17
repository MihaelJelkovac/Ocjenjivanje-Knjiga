using Lab4.Data;
using Lab4.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab4.Services;

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
            .Where(a => a.DeletedAt == null)
            .Include(a => a.Books)
            .OrderBy(author => author.LastName)
            .ThenBy(author => author.FirstName)
            .ToListAsync();
    }

    public async Task<Author?> GetByIdAsync(int id)
    {
        return await _context.Authors
            .Where(a => a.DeletedAt == null)
            .Include(a => a.Books)
            .FirstOrDefaultAsync(item => item.Id == id);
    }

    public async Task<Author> CreateAsync(Author author)
    {
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();
        return author;
    }

    public async Task<bool> UpdateAsync(Author author)
    {
        var existingAuthor = await _context.Authors.FindAsync(author.Id);
        if (existingAuthor == null)
            return false;

        _context.Entry(existingAuthor).CurrentValues.SetValues(author);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var author = await _context.Authors.FindAsync(id);
        if (author == null)
            return false;

        author.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyList<Author>> SearchAsync(string query)
    {
        return await _context.Authors
            .Where(a => a.DeletedAt == null && 
                (a.FirstName.Contains(query) || a.LastName.Contains(query)))
            .Take(20)
            .ToListAsync();
    }
}
