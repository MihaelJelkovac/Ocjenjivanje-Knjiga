using Lab2.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.Controllers;

public class AuthorsController : Controller
{
    private readonly IAuthorRepository _authorRepository;

    public AuthorsController(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    public async Task<IActionResult> Index()
    {
        var authors = await _authorRepository.GetAllAsync();
        return View(authors);
    }

    public async Task<IActionResult> Details(int id)
    {
        var author = await _authorRepository.GetByIdAsync(id);
        if (author is null)
        {
            return NotFound();
        }

        return View(author);
    }
}
