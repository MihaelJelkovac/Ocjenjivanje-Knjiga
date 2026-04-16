using Lab2.Data;
using Lab2.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab2.Services;

public class PublisherRepository : IPublisherRepository
{
    private readonly CatalogDbContext _context;

    public PublisherRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Publisher>> GetAllAsync()
    {
        var publishers = await _context.Publishers
            .AsNoTracking()
            .OrderBy(publisher => publisher.Name)
            .ToListAsync();

        return publishers;
    }

    public async Task<Publisher?> GetByIdAsync(int id)
    {
        return await _context.Publishers
            .AsNoTracking()
            .Include(publisher => publisher.Books)
            .SingleOrDefaultAsync(item => item.Id == id);
    }
}
