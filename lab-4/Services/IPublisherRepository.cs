using Lab4.Models;

namespace Lab4.Services;

public interface IPublisherRepository
{
    Task<IReadOnlyList<Publisher>> GetAllAsync();

    Task<Publisher?> GetByIdAsync(int id);

    Task<Publisher> CreateAsync(Publisher publisher);

    Task<bool> UpdateAsync(Publisher publisher);

    Task<bool> DeleteAsync(int id);

    Task<IReadOnlyList<Publisher>> SearchAsync(string query);
}
