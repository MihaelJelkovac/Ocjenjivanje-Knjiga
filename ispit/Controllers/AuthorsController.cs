using Lab5.Models;
using Lab5.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab5.Controllers;

[Route("[controller]")]
[Route("autori")]
public class AuthorsController : Controller
{
    private readonly IAuthorRepository _repository;
    private readonly ILogger<AuthorsController> _logger;

    public AuthorsController(IAuthorRepository repository, ILogger<AuthorsController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [AllowAnonymous]
    [Route("")]
    [Route("index")]
    public async Task<IActionResult> Index()
    {
        var authors = await _repository.GetAllAsync();
        return View(authors);
    }

    [AllowAnonymous]
    [Route("{id:int}")]
    [Route("profil/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var author = await _repository.GetByIdAsync(id);
        if (author is null)
        {
            return NotFound();
        }

        return View(author);
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
    public async Task<IActionResult> Create(Author model)
    {
        _logger.LogInformation("📝 Pokušaj kreiranja novog autora: {FirstName} {LastName}", model.FirstName, model.LastName);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("⚠️ Validacija nije prošla pri kreiranju autora: {FirstName} {LastName}", model.FirstName, model.LastName);
            return View(model);
        }

        try
        {
            await _repository.CreateAsync(model);
            _logger.LogInformation("✅ Autor uspješno kreiran: {FirstName} {LastName}", model.FirstName, model.LastName);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Greška pri kreiranju autora: {FirstName} {LastName}", model.FirstName, model.LastName);
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
        var author = await _repository.GetByIdAsync(id);
        if (author == null)
        {
            return NotFound();
        }

        return View("Edit", author);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost]
    [Route("edit/{id:int}")]
    [ActionName("Edit")]
    public async Task<IActionResult> EditPost(int id)
    {
        var author = await _repository.GetByIdAsync(id);
        if (author == null)
        {
            return NotFound();

        }

        var updateOk = await TryUpdateModelAsync(author);

        if (!updateOk || !ModelState.IsValid)
        {
            return View("Edit", author);
        }

        try
        {
            var success = await _repository.UpdateAsync(author);
            if (!success)
            {
                throw new Exception("Neuspješno ažuriranje");
            }

            _logger.LogInformation("✅ Autor uspješno ažuriran: {AuthorId}", author.Id);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Greška pri ažuriranju autora: {AuthorId}", author.Id);
            ModelState.AddModelError(string.Empty, "Greška pri ažuriranju: " + ex.Message);
            return View("Edit", author);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [Route("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("🗑️ Pokušaj brisanja autora ID: {AuthorId}", id);

        try
        {
            var success = await _repository.DeleteAsync(id);

            if (!success)
            {
                _logger.LogWarning("⚠️ Autor nije pronađen: {AuthorId}", id);
                return Json(new { success = false, message = "Autor nije pronađen" });
            }

            _logger.LogInformation("✅ Autor uspješno obrisan: {AuthorId}", id);
            return Json(new { success = true, message = "Autor je uspješno obrisan" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Greška pri brisanju autora: {AuthorId}", id);
            return Json(new { success = false, message = "Greška pri brisanju: " + ex.Message });
        }
    }

    [AllowAnonymous]
    [Route("search")]
    public async Task<IActionResult> Search(string query)
    {
        var results = await _repository.GetAllAsync();
        return Json(results
            .Where(a => a.FirstName.Contains(query ?? string.Empty, StringComparison.OrdinalIgnoreCase) ||
                       a.LastName.Contains(query ?? string.Empty, StringComparison.OrdinalIgnoreCase))
            .Take(50)
            .Select(a => new { id = a.Id, text = $"{a.FirstName} {a.LastName}" }));
    }
}

