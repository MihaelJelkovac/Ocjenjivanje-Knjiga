using Lab2.Models;

namespace Lab2.Services;

public class UserRepository : IUserRepository
{
    private readonly CatalogMockStore _store;

    public UserRepository(CatalogMockStore store)
    {
        _store = store;
    }

    public Task<IReadOnlyList<User>> GetAllAsync()
    {
        IReadOnlyList<User> users = _store.Users
            .OrderByDescending(user => user.ReputationPoints)
            .ThenBy(user => user.FullName)
            .ToList();

        return Task.FromResult(users);
    }

    public Task<User?> GetByIdAsync(int id)
    {
        var user = _store.Users.SingleOrDefault(item => item.Id == id);
        return Task.FromResult(user);
    }
}
