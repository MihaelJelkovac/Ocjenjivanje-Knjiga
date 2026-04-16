using Lab2.Models;

namespace Lab2.Services;

public interface IAuthorRepository
{
    Task<IReadOnlyList<Author>> GetAllAsync();

    Task<Author?> GetByIdAsync(int id);
}
