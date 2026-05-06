using Lab3.Data;
using Lab3.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab3.Services;

public class GenreRepository : IGenreRepository
{
    private readonly CatalogDbContext _context;

    public GenreRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Genre>> GetAllAsync()
    {
        return await _context.Genres
            .Include(g => g.BookGenres)
            .ThenInclude(bg => bg.Book)
            .OrderBy(genre => genre.Name)
            .ToListAsync();
    }

    public async Task<Genre?> GetByIdAsync(int id)
    {
        return await _context.Genres
            .Include(g => g.BookGenres)
            .ThenInclude(bg => bg.Book)
            .FirstOrDefaultAsync(item => item.Id == id);
    }
}
