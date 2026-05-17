using Lab4.Models;
using Lab4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab4.Controllers;

[Route("[controller]")]
[Route("autori")]
public class AuthorsController : Controller
{
    private readonly IAuthorRepository _repository;

    public AuthorsController(IAuthorRepository repository)
    {
        _repository = repository;
    }

    [Route("")]
    [Route("index")]
    public async Task<IActionResult> Index()
    {
        var authors = await _repository.GetAllAsync();
        return View(authors);
    }

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

    [HttpGet]
    [Route("create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create(Author model)
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
        var author = await _repository.GetByIdAsync(id);
        if (author == null)
        {
            return NotFound();
        }

        return View("Edit", author);
    }

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

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Greška pri ažuriranju: " + ex.Message);
            return View("Edit", author);
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
                return Json(new { success = false, message = "Autor nije pronađen" });
            }

            return Json(new { success = true, message = "Autor je uspješno obrisan" });
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
        return Json(results.Select(a => new { id = a.Id, text = $"{a.FirstName} {a.LastName}" }));
    }
}
