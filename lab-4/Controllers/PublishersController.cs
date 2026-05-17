using Lab4.Models;
using Lab4.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab4.Controllers;

[Route("[controller]")]
[Route("izdavaci")]
public class PublishersController : Controller
{
    private readonly IPublisherRepository _repository;

    public PublishersController(IPublisherRepository repository)
    {
        _repository = repository;
    }

    [Route("")]
    [Route("index")]
    public async Task<IActionResult> Index()
    {
        var publishers = await _repository.GetAllAsync();
        return View(publishers);
    }

    [Route("{id:int}")]
    [Route("detalji/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var publisher = await _repository.GetByIdAsync(id);
        if (publisher is null)
        {
            return NotFound();
        }

        return View(publisher);
    }

    [HttpGet]
    [Route("create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create(Publisher model)
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
        var publisher = await _repository.GetByIdAsync(id);
        if (publisher == null)
        {
            return NotFound();
        }

        return View("Edit", publisher);
    }

    [HttpPost]
    [Route("edit/{id:int}")]
    [ActionName("Edit")]
    public async Task<IActionResult> EditPost(int id)
    {
        var publisher = await _repository.GetByIdAsync(id);
        if (publisher == null)
        {
            return NotFound();
        }

        var updateOk = await TryUpdateModelAsync(publisher);

        if (!updateOk || !ModelState.IsValid)
        {
            return View("Edit", publisher);
        }

        try
        {
            var success = await _repository.UpdateAsync(publisher);
            if (!success)
            {
                throw new Exception("Neuspješno ažuriranje");
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Greška pri ažuriranju: " + ex.Message);
            return View("Edit", publisher);
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
                return Json(new { success = false, message = "Izdavač nije pronađen" });
            }

            return Json(new { success = true, message = "Izdavač je uspješno obrisan" });
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
        return Json(results.Select(p => new { id = p.Id, text = p.Name }));
    }
}
