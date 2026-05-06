using Lab3.Models;

namespace Lab3.Services;

public interface IReviewRepository
{
    Task<IReadOnlyList<Review>> GetAllAsync();

    Task<Review?> GetByIdAsync(int id);
}
