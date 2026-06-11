using Lab5.Models;

namespace Lab5.Services;

public interface IBookRepository
{
    Task<IReadOnlyList<Book>> GetAllAsync();

    /// <summary>
    /// Dohvata sve knjige na koje korisnik ima autorizaciju
    /// </summary>
    Task<IReadOnlyList<Book>> GetAllAsyncForUserAsync(string? appUserId);

    Task<Book?> GetByIdAsync(int id);

    /// <summary>
    /// Dohvata specifičnu knjiga za korisnika (provjerava autorizaciju)
    /// </summary>
    Task<Book?> GetByIdForUserAsync(int id, string? appUserId);

    Task<Book> CreateAsync(Book book);

    Task<bool> UpdateAsync(Book book);

    Task<bool> DeleteAsync(int id);
}

