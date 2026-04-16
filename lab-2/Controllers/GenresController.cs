using Lab2.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.Controllers;

public class GenresController : Controller
{
    private readonly IGenreRepository _genreRepository;

    public GenresController(IGenreRepository genreRepository)
    {
        _genreRepository = genreRepository;
    }

    public async Task<IActionResult> Index()
    {
        var genres = await _genreRepository.GetAllAsync();
        return View(genres);
    }

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
