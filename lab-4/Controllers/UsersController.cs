using Lab4.Models;
using Lab4.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab4.Controllers;

[Route("[controller]")]
[Route("korisnici")]
public class UsersController : Controller
{
    private readonly IUserRepository _repository;

    public UsersController(IUserRepository repository)
    {
        _repository = repository;
    }

    [Route("")]
    [Route("index")]
    public async Task<IActionResult> Index()
    {
        var users = await _repository.GetAllAsync();
        return View(users);
    }

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

    [HttpGet]
    [Route("create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create(User model)
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
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View("Edit", user);
    }

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

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Greška pri ažuriranju: " + ex.Message);
            return View("Edit", user);
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
                return Json(new { success = false, message = "Korisnik nije pronađen" });
            }

            return Json(new { success = true, message = "Korisnik je uspješno obrisan" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Greška pri brisanju: " + ex.Message });
        }
    }

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
