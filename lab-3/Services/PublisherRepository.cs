using Lab3.Data;
using Lab3.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab3.Services;

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
            .Include(p => p.Books)
            .OrderBy(publisher => publisher.Name)
            .ToListAsync();
    }

    public async Task<Publisher?> GetByIdAsync(int id)
    {
        // For Details view: Load all related Books
        return await _context.Publishers
            .Include(p => p.Books)
            .FirstOrDefaultAsync(item => item.Id == id);
    }
}
