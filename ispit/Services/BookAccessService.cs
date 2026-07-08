using Lab5.Data;
using Lab5.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab5.Services;

public class BookAccessService : IBookAccessService
{
    private readonly CatalogDbContext _context;

    public BookAccessService(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Book>> GetAuthorizedBooksAsync(string appUserId)
    {
        // Dohvati sve knjige na koje korisnik ima dozvolu (nije "Denied" i nije istekla)
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
            .OrderBy(b => b.Title)
            .ToListAsync();

        return books;
    }

    public async Task<bool> HasAccessAsync(string appUserId, int bookId)
    {
        var access = await _context.BookAccesses
            .FirstOrDefaultAsync(ba => ba.AppUserId == appUserId &&
                                       ba.BookId == bookId &&
                                       ba.AccessLevel != "Denied" &&
                                       ba.DeletedAt == null &&
                                       (ba.ExpiresAt == null || ba.ExpiresAt > DateTime.UtcNow));

        return access != null;
    }

    public async Task<bool> CanWriteAsync(string appUserId, int bookId)
    {
        var access = await _context.BookAccesses
            .FirstOrDefaultAsync(ba => ba.AppUserId == appUserId &&
                                       ba.BookId == bookId &&
                                       ba.AccessLevel == "ReadWrite" &&
                                       ba.DeletedAt == null &&
                                       (ba.ExpiresAt == null || ba.ExpiresAt > DateTime.UtcNow));

        return access != null;
    }

    public async Task GrantAccessAsync(string appUserId, int bookId, string accessLevel = "Read",
        string? grantedBy = null, string? reason = null, DateTime? expiresAt = null)
    {
        // Provjeri da li korisnik ili knjiga postoje
        var user = await _context.Users.FindAsync(appUserId);
        var book = await _context.Books.FindAsync(bookId);

        if (user == null || book == null)
            throw new InvalidOperationException("Korisnik ili knjiga ne postoje");

        // Provjeri da li već postoji dozvola
        var existingAccess = await _context.BookAccesses
            .FirstOrDefaultAsync(ba => ba.AppUserId == appUserId &&
                                       ba.BookId == bookId &&
                                       ba.DeletedAt == null);

        if (existingAccess != null)
        {
            // Ažuriraj existing dozvolu
            existingAccess.AccessLevel = accessLevel;
            existingAccess.ExpiresAt = expiresAt;
            existingAccess.GrantedAt = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(grantedBy))
                existingAccess.GrantedBy = grantedBy;
            if (!string.IsNullOrEmpty(reason))
                existingAccess.Reason = reason;
        }
        else
        {
            // Kreiraj novu dozvolu
            var newAccess = new BookAccess
            {
                AppUserId = appUserId,
                BookId = bookId,
                AccessLevel = accessLevel,
                GrantedAt = DateTime.UtcNow,
                GrantedBy = grantedBy,
                Reason = reason,
                ExpiresAt = expiresAt
            };

            _context.BookAccesses.Add(newAccess);
        }

        await _context.SaveChangesAsync();
    }

    public async Task RevokeAccessAsync(string appUserId, int bookId)
    {
        var access = await _context.BookAccesses
            .FirstOrDefaultAsync(ba => ba.AppUserId == appUserId &&
                                       ba.BookId == bookId &&
                                       ba.DeletedAt == null);

        if (access != null)
        {
            access.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateAccessAsync(string appUserId, int bookId, string newAccessLevel)
    {
        var access = await _context.BookAccesses
            .FirstOrDefaultAsync(ba => ba.AppUserId == appUserId &&
                                       ba.BookId == bookId &&
                                       ba.DeletedAt == null);

        if (access == null)
            throw new InvalidOperationException("Dozvola ne postoji");

        access.AccessLevel = newAccessLevel;
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<BookAccess>> GetBookAccessesAsync(int bookId)
    {
        var accesses = await _context.BookAccesses
            .Where(ba => ba.BookId == bookId &&
                         ba.DeletedAt == null)
            .Include(ba => ba.User)
            .OrderBy(ba => ba.GrantedAt)
            .ToListAsync();

        return accesses;
    }
}
