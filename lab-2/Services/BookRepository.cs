using Lab2.Models;

namespace Lab2.Services;

public class BookRepository : IBookRepository
{
    private readonly CatalogMockStore _store;

    public BookRepository(CatalogMockStore store)
    {
        _store = store;
    }

    public Task<IReadOnlyList<Book>> GetAllAsync()
    {
        IReadOnlyList<Book> books = _store.Books
            .OrderBy(book => book.Title)
            .ToList();

        return Task.FromResult(books);
    }

    public Task<Book?> GetByIdAsync(int id)
    {
        var book = _store.Books.SingleOrDefault(item => item.Id == id);
        return Task.FromResult(book);
    }
}
