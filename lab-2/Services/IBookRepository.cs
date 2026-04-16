using Lab2.Models;

namespace Lab2.Services;

public interface IBookRepository
{
    Task<IReadOnlyList<Book>> GetAllAsync();

    Task<Book?> GetByIdAsync(int id);
}
