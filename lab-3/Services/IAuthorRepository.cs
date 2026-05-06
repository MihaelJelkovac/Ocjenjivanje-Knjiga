using Lab3.Models;

namespace Lab3.Services;

public interface IAuthorRepository
{
    Task<IReadOnlyList<Author>> GetAllAsync();

    Task<Author?> GetByIdAsync(int id);
}
