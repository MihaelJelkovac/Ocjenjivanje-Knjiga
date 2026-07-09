using Lab5.Models;
using Lab5.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab5.Controllers;

[Route("[controller]")]
[Route("korisnici")]
public class UsersController : Controller
{
    private readonly IUserRepository _repository;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserRepository repository, ILogger<UsersController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [AllowAnonymous]
    [Route("")]
    [Route("index")]
    public async Task<IActionResult> Index()
    {
        var users = await _repository.GetAllAsync();
        return View(users);
    }

    [AllowAnonymous]
    [Route("{id:int}")]
    [Route("profil/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        return View(user);
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
    public async Task<IActionResult> Create(User model)
    {
        _logger.LogInformation("📝 Pokušaj kreiranja novog korisnika: {Username}", model.Username);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("⚠️ Validacija nije prošla pri kreiranju korisnika: {Username}", model.Username);
            return View(model);
        }

        try
        {
            await _repository.CreateAsync(model);
            _logger.LogInformation("✅ Korisnik uspješno kreiran: {Username}", model.Username);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Greška pri kreiranju korisnika: {Username}", model.Username);
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
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View("Edit", user);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost]
    [Route("edit/{id:int}")]
    [ActionName("Edit")]
    public async Task<IActionResult> EditPost(int id)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var updateOk = await TryUpdateModelAsync(user);

        if (!updateOk || !ModelState.IsValid)
        {
            return View("Edit", user);
        }

        try
        {
            var success = await _repository.UpdateAsync(user);
            if (!success)
            {
                throw new Exception("Neuspješno ažuriranje");
            }

            _logger.LogInformation("✅ Korisnik uspješno ažuriran: {UserId}", user.Id);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Greška pri ažuriranju korisnika: {UserId}", user.Id);
            ModelState.AddModelError(string.Empty, "Greška pri ažuriranju: " + ex.Message);
            return View("Edit", user);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [Route("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("🗑️ Pokušaj brisanja korisnika ID: {UserId}", id);

        try
        {
            var success = await _repository.DeleteAsync(id);

            if (!success)
            {
                _logger.LogWarning("⚠️ Korisnik nije pronađen: {UserId}", id);
                return Json(new { success = false, message = "Korisnik nije pronađen" });
            }

            _logger.LogInformation("✅ Korisnik uspješno obrisan: {UserId}", id);
            return Json(new { success = true, message = "Korisnik je uspješno obrisan" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Greška pri brisanju korisnika: {UserId}", id);
            return Json(new { success = false, message = "Greška pri brisanju: " + ex.Message });
        }
    }

    [AllowAnonymous]
    [Route("search")]
    public async Task<IActionResult> Search(string query)
    {
        var results = await _repository.GetAllAsync();
        return Json(results
            .Where(u =>
                u.FullName.Contains(query ?? string.Empty, StringComparison.OrdinalIgnoreCase) ||
                u.Username.Contains(query ?? string.Empty, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains(query ?? string.Empty, StringComparison.OrdinalIgnoreCase))
            .Take(50)
            .Select(u => new { id = u.Id, text = u.FullName }));
    }
}

