using Lab2.Models;

namespace Lab2.Services;

public interface IGenreRepository
{
    Task<IReadOnlyList<Genre>> GetAllAsync();

    Task<Genre?> GetByIdAsync(int id);
}
