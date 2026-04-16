using Lab2.Models;

namespace Lab2.Services;

public class AuthorRepository : IAuthorRepository
{
    private readonly CatalogMockStore _store;

    public AuthorRepository(CatalogMockStore store)
    {
        _store = store;
    }

    public Task<IReadOnlyList<Author>> GetAllAsync()
    {
        IReadOnlyList<Author> authors = _store.Authors
            .OrderBy(author => author.LastName)
            .ThenBy(author => author.FirstName)
            .ToList();

        return Task.FromResult(authors);
    }

    public Task<Author?> GetByIdAsync(int id)
    {
        var author = _store.Authors.SingleOrDefault(item => item.Id == id);
        return Task.FromResult(author);
    }
}
