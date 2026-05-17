using Lab4.Models;

namespace Lab4.Services;

public interface IGenreRepository
{
    Task<IReadOnlyList<Genre>> GetAllAsync();

    Task<Genre?> GetByIdAsync(int id);

    Task<Genre> CreateAsync(Genre genre);

    Task<bool> UpdateAsync(Genre genre);

    Task<bool> DeleteAsync(int id);

    Task<IReadOnlyList<Genre>> SearchAsync(string query);
}
