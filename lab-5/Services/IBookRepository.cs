using Lab5.Models;

namespace Lab5.Services;

public interface IBookRepository
{
    Task<IReadOnlyList<Book>> GetAllAsync();

    Task<Book?> GetByIdAsync(int id);

    Task<Book> CreateAsync(Book book);

    Task<bool> UpdateAsync(Book book);

    Task<bool> DeleteAsync(int id);
}

