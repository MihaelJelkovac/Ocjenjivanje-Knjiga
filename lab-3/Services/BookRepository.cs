using Lab3.Data;
using Lab3.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab3.Services;

public class BookRepository : IBookRepository
{
    private readonly CatalogDbContext _context;

    public BookRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Book>> GetAllAsync()
    {
        // For Index/List view: Load Author, Publisher, and Genres (without full Review details)
        return await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Publisher)
            .Include(b => b.BookGenres)
            .ThenInclude(bg => bg.Genre)
            .Include(b => b.Reviews)
            .OrderBy(book => book.Title)
            .ToListAsync();
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        // For Details view: Load all related data including Review user info
        return await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Publisher)
            .Include(b => b.BookGenres)
            .ThenInclude(bg => bg.Genre)
            .Include(b => b.Reviews)
            .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(item => item.Id == id);
    }
}
