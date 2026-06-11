using Lab5.Data;
using Lab5.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab5.Services;

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
            .Where(b => b.DeletedAt == null)
            .Include(b => b.Author)
            .Include(b => b.Publisher)
            .Include(b => b.BookGenres)
            .ThenInclude(bg => bg.Genre)
            .Include(b => b.Reviews)
            .OrderBy(book => book.Title)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Book>> GetAllAsyncForUserAsync(string? appUserId)
    {
        // Ako nema appUserId (anoniman), vrati sve knjige
        if (string.IsNullOrEmpty(appUserId))
        {
            return await GetAllAsync();
        }

        // Dohvati sve knjige na koje korisnik ima dozvolu
        var authorizedBookIds = await _context.BookAccesses
            .Where(ba => ba.AppUserId == appUserId &&
                         ba.AccessLevel != "Denied" &&
                         ba.DeletedAt == null &&
                         (ba.ExpiresAt == null || ba.ExpiresAt > DateTime.UtcNow))
            .Select(ba => ba.BookId)
            .ToListAsync();

        var books = await _context.Books
            .Where(b => b.DeletedAt == null && authorizedBookIds.Contains(b.Id))
            .Include(b => b.Author)
            .Include(b => b.Publisher)
            .Include(b => b.BookGenres)
            .ThenInclude(bg => bg.Genre)
            .Include(b => b.Reviews)
            .OrderBy(book => book.Title)
            .ToListAsync();

        return books;
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        // For Details view: Load all related data including Review user info
        return await _context.Books
            .Where(b => b.DeletedAt == null)
            .Include(b => b.Author)
            .Include(b => b.Publisher)
            .Include(b => b.BookGenres)
            .ThenInclude(bg => bg.Genre)
            .Include(b => b.Reviews)
            .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(item => item.Id == id);
    }

    public async Task<Book?> GetByIdForUserAsync(int id, string? appUserId)
    {
        // Ako nema appUserId (anoniman), vrati Book bez provere
        if (string.IsNullOrEmpty(appUserId))
        {
            return await GetByIdAsync(id);
        }

        // Provjeri da li korisnik ima dozvolu za ovu knjugu
        var hasAccess = await _context.BookAccesses
            .AnyAsync(ba => ba.AppUserId == appUserId &&
                            ba.BookId == id &&
                            ba.AccessLevel != "Denied" &&
                            ba.DeletedAt == null &&
                            (ba.ExpiresAt == null || ba.ExpiresAt > DateTime.UtcNow));

        if (!hasAccess)
            return null;

        return await GetByIdAsync(id);
    }

    public async Task<Book> CreateAsync(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return book;
    }

    public async Task<bool> UpdateAsync(Book book)
    {
        var existingBook = await _context.Books.FindAsync(book.Id);
        if (existingBook == null)
            return false;

        _context.Entry(existingBook).CurrentValues.SetValues(book);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
            return false;

        book.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}

