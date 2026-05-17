using Lab4.Models;

namespace Lab4.Services;

public interface IUserRepository
{
    Task<IReadOnlyList<User>> GetAllAsync();

    Task<User?> GetByIdAsync(int id);

    Task<User> CreateAsync(User user);

    Task<bool> UpdateAsync(User user);

    Task<bool> DeleteAsync(int id);
}
