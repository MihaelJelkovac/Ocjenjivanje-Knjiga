using Lab3.Models;

namespace Lab3.Services;

public interface IGenreRepository
{
    Task<IReadOnlyList<Genre>> GetAllAsync();

    Task<Genre?> GetByIdAsync(int id);
}
