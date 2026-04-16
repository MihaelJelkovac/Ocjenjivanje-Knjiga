using Lab2.Models;

namespace Lab2.Services;

public interface IUserRepository
{
    Task<IReadOnlyList<User>> GetAllAsync();

    Task<User?> GetByIdAsync(int id);
}
