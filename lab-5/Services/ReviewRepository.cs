using Lab5.Data;
using Lab5.Models;
using Lab5.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Lab5.Services;

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
            .Where(r => r.DeletedAt == null)
            .Include(r => r.Book)
            .Include(r => r.User)
            .OrderByDescending(review => review.ReviewedAt)
            .ToListAsync();
    }

    public async Task<Review?> GetByIdAsync(int id)
    {
        return await _context.Reviews
            .Where(r => r.DeletedAt == null)
            .Include(r => r.Book)
            .Include(r => r.User)
            .FirstOrDefaultAsync(item => item.Id == id);
    }

    public async Task<Review> CreateAsync(Review review)
    {
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
        return review;
    }

    public async Task<bool> UpdateAsync(Review review)
    {
        var existingReview = await _context.Reviews.FindAsync(review.Id);
        if (existingReview == null)
            return false;

        _context.Entry(existingReview).CurrentValues.SetValues(review);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            return false;

        review.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}

