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
        // For Index/List view: Load minimal BookGenres (no full Book data needed)
        return await _context.Genres
            .Include(g => g.BookGenres)
            .OrderBy(genre => genre.Name)
            .ToListAsync();
    }

    public async Task<Genre?> GetByIdAsync(int id)
    {
        // For Details view: Load all related Books in genre
        return await _context.Genres
            .Include(g => g.BookGenres)
            .ThenInclude(bg => bg.Book)
            .FirstOrDefaultAsync(item => item.Id == id);
    }
}
