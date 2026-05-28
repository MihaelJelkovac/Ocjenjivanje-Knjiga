using Lab5.Models;

namespace Lab5.Services;

public interface IReviewRepository
{
    Task<IReadOnlyList<Review>> GetAllAsync();

    Task<Review?> GetByIdAsync(int id);

    Task<Review> CreateAsync(Review review);

    Task<bool> UpdateAsync(Review review);

    Task<bool> DeleteAsync(int id);
}

