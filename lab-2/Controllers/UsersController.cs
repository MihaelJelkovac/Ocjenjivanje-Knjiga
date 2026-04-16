using Lab2.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.Controllers;

public class UsersController : Controller
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userRepository.GetAllAsync();
        return View(users);
    }

    public async Task<IActionResult> Details(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        return View(user);
    }
}
