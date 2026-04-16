using Lab2.Models;

namespace Lab2.Services;

public class ReviewRepository : IReviewRepository
{
    private readonly CatalogMockStore _store;

    public ReviewRepository(CatalogMockStore store)
    {
        _store = store;
    }

    public Task<IReadOnlyList<Review>> GetAllAsync()
    {
        IReadOnlyList<Review> reviews = _store.Reviews
            .OrderByDescending(review => review.ReviewedAt)
            .ToList();

        return Task.FromResult(reviews);
    }

    public Task<Review?> GetByIdAsync(int id)
    {
        var review = _store.Reviews.SingleOrDefault(item => item.Id == id);
        return Task.FromResult(review);
    }
}
