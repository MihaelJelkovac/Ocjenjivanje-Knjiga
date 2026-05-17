using Lab4.Models;
using Lab4.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab4.Controllers;

[Route("[controller]")]
[Route("zanrovi")]
public class GenresController : Controller
{
    private readonly IGenreRepository _repository;

    public GenresController(IGenreRepository repository)
    {
        _repository = repository;
    }

    [Route("")]
    [Route("index")]
    public async Task<IActionResult> Index()
    {
        var genres = await _repository.GetAllAsync();
        return View(genres);
    }

    [Route("{id:int}")]
    [Route("popis/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var genre = await _repository.GetByIdAsync(id);
        if (genre is null)
        {
            return NotFound();
        }

        return View(genre);
    }

    [HttpGet]
    [Route("create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create(Genre model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _repository.CreateAsync(model);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Greška pri spremanju: " + ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    [Route("edit/{id:int}")]
    [ActionName("Edit")]
    public async Task<IActionResult> EditGet(int id)
    {
        var genre = await _repository.GetByIdAsync(id);
        if (genre == null)
        {
            return NotFound();
        }

        return View("Edit", genre);
    }

    [HttpPost]
    [Route("edit/{id:int}")]
    [ActionName("Edit")]
    public async Task<IActionResult> EditPost(int id)
    {
        var genre = await _repository.GetByIdAsync(id);
        if (genre == null)
        {
            return NotFound();
        }

        var updateOk = await TryUpdateModelAsync(genre);

        if (!updateOk || !ModelState.IsValid)
        {
            return View("Edit", genre);
        }

        try
        {
            var success = await _repository.UpdateAsync(genre);
            if (!success)
            {
                throw new Exception("Neuspješno ažuriranje");
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Greška pri ažuriranju: " + ex.Message);
            return View("Edit", genre);
        }
    }

    [HttpPost]
    [Route("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var success = await _repository.DeleteAsync(id);

            if (!success)
            {
                return Json(new { success = false, message = "ŽAnr nije pronađen" });
            }

            return Json(new { success = true, message = "ŽAnr je uspješno obrisan" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Greška pri brisanju: " + ex.Message });
        }
    }

    [Route("search")]
    public async Task<IActionResult> Search(string query)
    {
        var results = await _repository.SearchAsync(query ?? string.Empty);
        return Json(results.Select(g => new { id = g.Id, text = g.Name }));
    }
}
