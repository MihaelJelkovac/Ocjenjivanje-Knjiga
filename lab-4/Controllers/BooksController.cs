using Lab4.Models;
using Lab4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab4.Controllers;

[Route("[controller]")]
[Route("knjige")]
public class BooksController : Controller
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IPublisherRepository _publisherRepository;
    private readonly IGenreRepository _genreRepository;

    public BooksController(
        IBookRepository bookRepository,
        IAuthorRepository authorRepository,
        IPublisherRepository publisherRepository,
        IGenreRepository genreRepository)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _publisherRepository = publisherRepository;
        _genreRepository = genreRepository;
    }

    [Route("")]
    [Route("index")]
    public async Task<IActionResult> Index()
    {
        var books = await _bookRepository.GetAllAsync();
        return View(books);
    }

    [Route("{id:int}")]
    [Route("detalji/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book is null)
        {
            return NotFound();
        }

        return View(book);
    }

    [HttpGet]
    [Route("create")]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View();
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create(Book model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync();
            return View(model);
        }

        try
        {
            await _bookRepository.CreateAsync(model);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Greška pri spremanju: " + ex.Message);
            await PopulateDropdownsAsync();
            return View(model);
        }
    }

    [HttpGet]
    [Route("edit/{id:int}")]
    [ActionName("Edit")]
    public async Task<IActionResult> EditGet(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        await PopulateDropdownsAsync(book);
        return View("Edit", book);
    }

    [HttpPost]
    [Route("edit/{id:int}")]
    [ActionName("Edit")]
    public async Task<IActionResult> EditPost(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        var updateOk = await TryUpdateModelAsync(book);

        if (!updateOk || !ModelState.IsValid)
        {
            await PopulateDropdownsAsync(book);
            return View("Edit", book);
        }

        try
        {
            var success = await _bookRepository.UpdateAsync(book);
            if (!success)
            {
                throw new Exception("Neuspješno ažuriranje");
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Greška pri ažuriranju: " + ex.Message);
            await PopulateDropdownsAsync(book);
            return View("Edit", book);
        }
    }

    [HttpPost]
    [Route("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var success = await _bookRepository.DeleteAsync(id);

            if (!success)
            {
                return Json(new { success = false, message = "Knjiga nije pronađena" });
            }

            return Json(new { success = true, message = "Knjiga je uspješno obrisana" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Greška pri brisanju: " + ex.Message });
        }
    }

    [Route("search")]
    public async Task<IActionResult> Search(string query)
    {
        var results = await _bookRepository.GetAllAsync();
        return Json(results
            .Where(b =>
                b.Title.Contains(query ?? string.Empty, StringComparison.OrdinalIgnoreCase) ||
                (b.Author != null && (b.Author.FirstName.Contains(query ?? string.Empty, StringComparison.OrdinalIgnoreCase) ||
                                     b.Author.LastName.Contains(query ?? string.Empty, StringComparison.OrdinalIgnoreCase))) ||
                (b.Publisher != null && b.Publisher.Name.Contains(query ?? string.Empty, StringComparison.OrdinalIgnoreCase)))
            .Take(50)
            .Select(b => new { id = b.Id, text = b.Title }));
    }

    private async Task PopulateDropdownsAsync(Book? book = null)
    {
        var authors = await _authorRepository.GetAllAsync();
        ViewBag.Authors = authors.Select(a => new SelectListItem
        {
            Value = a.Id.ToString(),
            Text = $"{a.FirstName} {a.LastName}",
            Selected = book != null && book.AuthorId == a.Id
        }).ToList();

        var publishers = await _publisherRepository.GetAllAsync();
        ViewBag.Publishers = publishers.Select(p => new SelectListItem
        {
            Value = p.Id.ToString(),
            Text = p.Name,
            Selected = book != null && book.PublisherId == p.Id
        }).ToList();

        var genres = await _genreRepository.GetAllAsync();
        ViewBag.Genres = genres.Select(g => new SelectListItem
        {
            Value = g.Id.ToString(),
            Text = g.Name
        }).ToList();

        ViewBag.Statuses = Enum.GetValues(typeof(BookStatus))
            .Cast<BookStatus>()
            .Select(s => new SelectListItem
            {
                Value = s.ToString(),
                Text = s.ToString(),
                Selected = book != null && book.Status == s
            })
            .ToList();
    }
}
