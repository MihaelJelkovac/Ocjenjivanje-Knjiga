using Lab4.Data;
using Lab4.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab4.Services;

public class PublisherRepository : IPublisherRepository
{
    private readonly CatalogDbContext _context;

    public PublisherRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Publisher>> GetAllAsync()
    {
        // For Index/List view: Load minimal Books (only for counting)
        return await _context.Publishers
            .Where(p => p.DeletedAt == null)
            .Include(p => p.Books)
            .OrderBy(publisher => publisher.Name)
            .ToListAsync();
    }

    public async Task<Publisher?> GetByIdAsync(int id)
    {
        // For Details view: Load all related Books
        return await _context.Publishers
            .Where(p => p.DeletedAt == null)
            .Include(p => p.Books)
            .FirstOrDefaultAsync(item => item.Id == id);
    }

    public async Task<Publisher> CreateAsync(Publisher publisher)
    {
        _context.Publishers.Add(publisher);
        await _context.SaveChangesAsync();
        return publisher;
    }

    public async Task<bool> UpdateAsync(Publisher publisher)
    {
        var existingPublisher = await _context.Publishers.FindAsync(publisher.Id);
        if (existingPublisher == null)
            return false;

        _context.Entry(existingPublisher).CurrentValues.SetValues(publisher);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var publisher = await _context.Publishers.FindAsync(id);
        if (publisher == null)
            return false;

        publisher.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyList<Publisher>> SearchAsync(string query)
    {
        return await _context.Publishers
            .Where(p => p.DeletedAt == null && p.Name.Contains(query))
            .Take(20)
            .ToListAsync();
    }
}
