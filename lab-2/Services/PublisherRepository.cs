using Lab2.Models;

namespace Lab2.Services;

public class PublisherRepository : IPublisherRepository
{
    private readonly CatalogMockStore _store;

    public PublisherRepository(CatalogMockStore store)
    {
        _store = store;
    }

    public Task<IReadOnlyList<Publisher>> GetAllAsync()
    {
        IReadOnlyList<Publisher> publishers = _store.Publishers
            .OrderBy(publisher => publisher.Name)
            .ToList();

        return Task.FromResult(publishers);
    }

    public Task<Publisher?> GetByIdAsync(int id)
    {
        var publisher = _store.Publishers.SingleOrDefault(item => item.Id == id);
        return Task.FromResult(publisher);
    }
}
