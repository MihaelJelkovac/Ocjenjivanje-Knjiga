using Lab2.Data;
using Lab2.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab2.Services;

public class BookRepository : IBookRepository
{
    private readonly CatalogDbContext _context;

    public BookRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Book>> GetAllAsync()
    {
        var books = await _context.Books
            .AsNoTracking()
            .Include(book => book.Author)
            .Include(book => book.Publisher)
            .Include(book => book.BookGenres)
                .ThenInclude(bookGenre => bookGenre.Genre)
            .Include(book => book.Reviews)
                .ThenInclude(review => review.User)
            .OrderBy(book => book.Title)
            .ToListAsync();

        return books;
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        return await _context.Books
            .AsNoTracking()
            .Include(book => book.Author)
            .Include(book => book.Publisher)
            .Include(book => book.BookGenres)
                .ThenInclude(bookGenre => bookGenre.Genre)
            .Include(book => book.Reviews)
                .ThenInclude(review => review.User)
            .SingleOrDefaultAsync(item => item.Id == id);
    }
}
