using Lab3.Data;
using Lab3.Models;
using Lab3.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Lab3.Services;

public class ReviewRepository : IReviewRepository
{
    private readonly CatalogDbContext _context;

    public ReviewRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Review>> GetAllAsync()
    {
        return await _context.Reviews
            .Include(r => r.Book)
            .Include(r => r.User)
            .OrderByDescending(review => review.ReviewedAt)
            .ToListAsync();
    }

    public async Task<Review?> GetByIdAsync(int id)
    {
        return await _context.Reviews
            .Include(r => r.Book)
            .Include(r => r.User)
            .FirstOrDefaultAsync(item => item.Id == id);
    }
}
