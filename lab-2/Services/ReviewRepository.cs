using Lab2.Data;
using Lab2.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab2.Services;

public class ReviewRepository : IReviewRepository
{
    private readonly CatalogDbContext _context;

    public ReviewRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Review>> GetAllAsync()
    {
        var reviews = await _context.Reviews
            .AsNoTracking()
            .Include(review => review.Book)
            .Include(review => review.User)
            .OrderByDescending(review => review.ReviewedAt)
            .ToListAsync();

        return reviews;
    }

    public async Task<Review?> GetByIdAsync(int id)
    {
        return await _context.Reviews
            .AsNoTracking()
            .Include(review => review.Book)
            .Include(review => review.User)
            .SingleOrDefaultAsync(item => item.Id == id);
    }
}
