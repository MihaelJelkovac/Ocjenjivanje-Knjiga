using Lab3.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab3.Controllers;

[Route("[controller]")]
[Route("korisnici")]
public class UsersController : Controller
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [Route("")]
    [Route("index")]
    public async Task<IActionResult> Index()
    {
        var users = await _userRepository.GetAllAsync();
        return View(users);
    }

    [Route("{id:int}")]
    [Route("profil/{id:int}")]
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
