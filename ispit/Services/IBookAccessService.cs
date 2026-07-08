using Lab5.Models;

namespace Lab5.Services;

/// <summary>
/// Definiše dozvole i pravila za pristup knjigama po korisniku
/// </summary>
public interface IBookAccessService
{
    /// <summary>
    /// Dohvata sve knjige na koje korisnik ima dozvolu
    /// </summary>
    Task<IReadOnlyList<Book>> GetAuthorizedBooksAsync(string appUserId);

    /// <summary>
    /// Provjerava da li korisnik ima dozvolu za specifičnu knjugu
    /// </summary>
    Task<bool> HasAccessAsync(string appUserId, int bookId);

    /// <summary>
    /// Provjerava da li korisnik može pisati/uređivati knjugu
    /// </summary>
    Task<bool> CanWriteAsync(string appUserId, int bookId);

    /// <summary>
    /// Daje dozvolu korisniku za pristup knjizi
    /// </summary>
    Task GrantAccessAsync(string appUserId, int bookId, string accessLevel = "Read",
        string? grantedBy = null, string? reason = null, DateTime? expiresAt = null);

    /// <summary>
    /// Oduzima dozvolu
    /// </summary>
    Task RevokeAccessAsync(string appUserId, int bookId);

    /// <summary>
    /// Ažurira dozvolu
    /// </summary>
    Task UpdateAccessAsync(string appUserId, int bookId, string newAccessLevel);

    /// <summary>
    /// Dohvata sve dozvole za specifičnu knjugu
    /// </summary>
    Task<IReadOnlyList<BookAccess>> GetBookAccessesAsync(int bookId);
}
