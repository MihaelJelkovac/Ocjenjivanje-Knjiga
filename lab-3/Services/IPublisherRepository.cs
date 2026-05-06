using Lab3.Models;

namespace Lab3.Services;

public interface IPublisherRepository
{
    Task<IReadOnlyList<Publisher>> GetAllAsync();

    Task<Publisher?> GetByIdAsync(int id);
}
