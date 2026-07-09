using Lab5.Models;
using Lab5.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab5.Controllers;

[Route("[controller]")]
[Route("zanrovi")]
public class GenresController : Controller
{
    private readonly IGenreRepository _repository;
    private readonly ILogger<GenresController> _logger;

    public GenresController(IGenreRepository repository, ILogger<GenresController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [AllowAnonymous]
    [Route("")]
    [Route("index")]
    public async Task<IActionResult> Index()
    {
        var genres = await _repository.GetAllAsync();
        return View(genres);
    }

    [AllowAnonymous]
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

    [Authorize(Roles = "Admin,Manager")]
    [HttpGet]
    [Route("create")]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create(Genre model)
    {
        _logger.LogInformation("📝 Pokušaj kreiranja novog žanra: {GenreName}", model.Name);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("⚠️ Validacija nije prošla pri kreiranju žanra: {GenreName}", model.Name);
            return View(model);
        }

        try
        {
            await _repository.CreateAsync(model);
            _logger.LogInformation("✅ Žanr uspješno kreiran: {GenreName}", model.Name);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Greška pri kreiranju žanra: {GenreName}", model.Name);
            ModelState.AddModelError(string.Empty, "Greška pri spremanju: " + ex.Message);
            return View(model);
        }
    }

    [Authorize(Roles = "Admin,Manager")]
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

    [Authorize(Roles = "Admin,Manager")]
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

            _logger.LogInformation("✅ Žanr uspješno ažuriran: {GenreId}", genre.Id);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Greška pri ažuriranju žanra: {GenreId}", genre.Id);
            ModelState.AddModelError(string.Empty, "Greška pri ažuriranju: " + ex.Message);
            return View("Edit", genre);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [Route("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("🗑️ Pokušaj brisanja žanra ID: {GenreId}", id);

        try
        {
            var success = await _repository.DeleteAsync(id);

            if (!success)
            {
                _logger.LogWarning("⚠️ Žanr nije pronađen: {GenreId}", id);
                return Json(new { success = false, message = "ŽAnr nije pronađen" });
            }

            _logger.LogInformation("✅ Žanr uspješno obrisan: {GenreId}", id);
            return Json(new { success = true, message = "ŽAnr je uspješno obrisan" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Greška pri brisanju žanra: {GenreId}", id);
            return Json(new { success = false, message = "Greška pri brisanju: " + ex.Message });
        }
    }

    [AllowAnonymous]
    [Route("search")]
    public async Task<IActionResult> Search(string query)
    {
        var results = await _repository.GetAllAsync();
        return Json(results
            .Where(g => g.Name.Contains(query ?? string.Empty, StringComparison.OrdinalIgnoreCase))
            .Take(50)
            .Select(g => new { id = g.Id, text = g.Name }));
    }
}

