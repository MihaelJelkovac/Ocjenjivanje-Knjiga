using Lab3.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab3.Controllers;

[Route("[controller]")]
[Route("autori")]
public class AuthorsController : Controller
{
    private readonly IAuthorRepository _authorRepository;

    public AuthorsController(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    [Route("")]
    [Route("index")]
    public async Task<IActionResult> Index()
    {
        var authors = await _authorRepository.GetAllAsync();
        return View(authors);
    }

    [Route("{id:int}")]
    [Route("profil/{id:int}")]
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
