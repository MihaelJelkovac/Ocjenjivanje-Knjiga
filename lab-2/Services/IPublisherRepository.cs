using Lab2.Models;

namespace Lab2.Services;

public interface IPublisherRepository
{
    Task<IReadOnlyList<Publisher>> GetAllAsync();

    Task<Publisher?> GetByIdAsync(int id);
}
