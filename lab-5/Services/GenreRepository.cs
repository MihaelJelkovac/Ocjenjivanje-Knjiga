using Lab5.Data;
using Lab5.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab5.Services;

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
            .Where(g => g.DeletedAt == null)
            .Include(g => g.BookGenres)
            .OrderBy(genre => genre.Name)
            .ToListAsync();
    }

    public async Task<Genre?> GetByIdAsync(int id)
    {
        // For Details view: Load all related Books in genre
        return await _context.Genres
            .Where(g => g.DeletedAt == null)
            .Include(g => g.BookGenres)
            .ThenInclude(bg => bg.Book)
            .FirstOrDefaultAsync(item => item.Id == id);
    }

    public async Task<Genre> CreateAsync(Genre genre)
    {
        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();
        return genre;
    }

    public async Task<bool> UpdateAsync(Genre genre)
    {
        var existingGenre = await _context.Genres.FindAsync(genre.Id);
        if (existingGenre == null)
            return false;

        _context.Entry(existingGenre).CurrentValues.SetValues(genre);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var genre = await _context.Genres.FindAsync(id);
        if (genre == null)
            return false;

        genre.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyList<Genre>> SearchAsync(string query)
    {
        var queryLower = query.ToLower();
        return await _context.Genres
            .Where(g => g.DeletedAt == null && g.Name.ToLower().Contains(queryLower))
            .Take(20)
            .ToListAsync();
    }
}

