using Lab3.Models;

namespace Lab3.Services;

public interface IBookRepository
{
    Task<IReadOnlyList<Book>> GetAllAsync();

    Task<Book?> GetByIdAsync(int id);
}
