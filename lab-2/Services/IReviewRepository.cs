using Lab2.Models;

namespace Lab2.Services;

public interface IReviewRepository
{
    Task<IReadOnlyList<Review>> GetAllAsync();

    Task<Review?> GetByIdAsync(int id);
}
