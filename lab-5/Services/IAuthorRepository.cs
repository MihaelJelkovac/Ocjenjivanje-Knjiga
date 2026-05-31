using Lab5.Models;

namespace Lab5.Services;

public interface IAuthorRepository
{
    Task<IReadOnlyList<Author>> GetAllAsync();

    Task<Author?> GetByIdAsync(int id);

    Task<Author> CreateAsync(Author author);

    Task<bool> UpdateAsync(Author author);

    Task<bool> DeleteAsync(int id);
}

