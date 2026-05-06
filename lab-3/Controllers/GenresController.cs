using Lab3.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab3.Controllers;

[Route("[controller]")]
[Route("zanrovi")]
public class GenresController : Controller
{
    private readonly IGenreRepository _genreRepository;

    public GenresController(IGenreRepository genreRepository)
    {
        _genreRepository = genreRepository;
    }

    [Route("")]
    [Route("index")]
    public async Task<IActionResult> Index()
    {
        var genres = await _genreRepository.GetAllAsync();
        return View(genres);
    }

    [Route("{id:int}")]
    [Route("popis/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var genre = await _genreRepository.GetByIdAsync(id);
        if (genre is null)
        {
            return NotFound();
        }

        return View(genre);
    }
}
