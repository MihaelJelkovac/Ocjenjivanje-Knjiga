using Lab3.Models;

namespace Lab3.Services;

public interface IUserRepository
{
    Task<IReadOnlyList<User>> GetAllAsync();

    Task<User?> GetByIdAsync(int id);
}
