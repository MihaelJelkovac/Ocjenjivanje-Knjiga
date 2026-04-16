using Lab2.Data;
using Lab2.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab2.Services;

public class GenreRepository : IGenreRepository
{
    private readonly CatalogDbContext _context;

    public GenreRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Genre>> GetAllAsync()
    {
        var genres = await _context.Genres
            .AsNoTracking()
            .OrderBy(genre => genre.Name)
            .ToListAsync();

        return genres;
    }

    public async Task<Genre?> GetByIdAsync(int id)
    {
        return await _context.Genres
            .AsNoTracking()
            .Include(genre => genre.BookGenres)
                .ThenInclude(bookGenre => bookGenre.Book)
            .SingleOrDefaultAsync(item => item.Id == id);
    }
}
