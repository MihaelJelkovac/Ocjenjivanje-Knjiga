using Lab5.Models;

namespace Lab5.Services;

public interface IGenreRepository
{
    Task<IReadOnlyList<Genre>> GetAllAsync();

    Task<Genre?> GetByIdAsync(int id);

    Task<Genre> CreateAsync(Genre genre);

    Task<bool> UpdateAsync(Genre genre);

    Task<bool> DeleteAsync(int id);
}

