using Lab2.Models;

namespace Lab2.Services;

public class GenreRepository : IGenreRepository
{
    private readonly CatalogMockStore _store;

    public GenreRepository(CatalogMockStore store)
    {
        _store = store;
    }

    public Task<IReadOnlyList<Genre>> GetAllAsync()
    {
        IReadOnlyList<Genre> genres = _store.Genres
            .OrderBy(genre => genre.Name)
            .ToList();

        return Task.FromResult(genres);
    }

    public Task<Genre?> GetByIdAsync(int id)
    {
        var genre = _store.Genres.SingleOrDefault(item => item.Id == id);
        return Task.FromResult(genre);
    }
}
