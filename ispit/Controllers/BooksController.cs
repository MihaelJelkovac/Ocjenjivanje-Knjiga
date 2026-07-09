using Lab5.Models;
using Lab5.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab5.Controllers;

[Route("[controller]")]
[Route("knjige")]
public class BooksController : Controller
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IPublisherRepository _publisherRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<BooksController> _logger;
    private readonly IAIService _aiService;

    public BooksController(
        IBookRepository bookRepository,
        IAuthorRepository authorRepository,
        IPublisherRepository publisherRepository,
        IGenreRepository genreRepository,
        UserManager<AppUser> userManager,
        ILogger<BooksController> logger,
        IAIService aiService)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _publisherRepository = publisherRepository;
        _genreRepository = genreRepository;
        _userManager = userManager;
        _logger = logger;
        _aiService = aiService;
    }

    [AllowAnonymous]
    [Route("")]
    [Route("index")]
    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("📖 Pristup Index akciji - Popis svih knjiga");
        var currentUser = await _userManager.GetUserAsync(User);
        var books = await _bookRepository.GetAllAsyncForUserAsync(currentUser?.Id);
        _logger.LogInformation("✅ Učitano {BookCount} knjiga", books.Count);
        return View(books);
    }

    [AllowAnonymous]
    [Route("{id:int}")]
    [Route("detalji/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var book = await _bookRepository.GetByIdForUserAsync(id, currentUser?.Id);
        if (book is null)
        {
            return NotFound();
        }

        return View(book);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpGet]
    [Route("create")]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View();
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create(Book model)
    {
        _logger.LogInformation("📝 Pokušaj kreiranja nove knjige: {BookTitle}", model.Title);

        // Try to resolve autocomplete display text to ids if the hidden id wasn't provided
        var authorSearch = Request.Form["AuthorId-search"].FirstOrDefault();
        if (model.AuthorId == 0 && !string.IsNullOrWhiteSpace(authorSearch))
        {
            var allAuthors = await _authorRepository.GetAllAsync();
            var exact = allAuthors.FirstOrDefault(a => ($"{a.FirstName} {a.LastName}").Equals(authorSearch, StringComparison.OrdinalIgnoreCase));
            if (exact != null)
            {
                model.AuthorId = exact.Id;
                ModelState.Remove("AuthorId");
            }
        }

        var publisherSearch = Request.Form["PublisherId-search"].FirstOrDefault();
        if (model.PublisherId == 0 && !string.IsNullOrWhiteSpace(publisherSearch))
        {
            var allPubs = await _publisherRepository.GetAllAsync();
            var exactPub = allPubs.FirstOrDefault(p => p.Name.Equals(publisherSearch, StringComparison.OrdinalIgnoreCase));
            if (exactPub != null)
            {
                model.PublisherId = exactPub.Id;
                ModelState.Remove("PublisherId");
            }
        }

        // Remove navigation property binding errors (we validate by Ids)
        ModelState.Remove("Author");
        ModelState.Remove("Publisher");

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("⚠️ Validacija nije prošla pri kreiranju: {BookTitle}", model.Title);
            await PopulateDropdownsAsync();
            return View(model);
        }

        try
        {
            await _bookRepository.CreateAsync(model);
            _logger.LogInformation("✅ Knjiga uspješno kreirana: {BookTitle}", model.Title);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Greška pri kreiranju knjige: {BookTitle}", model.Title);
            ModelState.AddModelError(string.Empty, "Greška pri spremanju: " + ex.Message);
            await PopulateDropdownsAsync();
            return View(model);
        }
    }

    [Authorize(Roles = "Admin,Manager")]
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

    [Authorize(Roles = "Admin,Manager")]
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

            _logger.LogInformation("✅ Knjiga uspješno ažurirana: {BookId} - {BookTitle}", book.Id, book.Title);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Greška pri ažuriranju knjige: {BookId}", book.Id);
            ModelState.AddModelError(string.Empty, "Greška pri ažuriranju: " + ex.Message);
            await PopulateDropdownsAsync(book);
            return View("Edit", book);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [Route("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("🗑️ Pokušaj brisanja knjige ID: {BookId}", id);

        try
        {
            var success = await _bookRepository.DeleteAsync(id);

            if (!success)
            {
                _logger.LogWarning("⚠️ Knjiga nije pronađena: {BookId}", id);
                return Json(new { success = false, message = "Knjiga nije pronađena" });
            }

            _logger.LogInformation("✅ Knjiga uspješno obrisana: {BookId}", id);
            return Json(new { success = true, message = "Knjiga je uspješno obrisana" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Greška pri brisanju knjige: {BookId}", id);
            return Json(new { success = false, message = "Greška pri brisanju: " + ex.Message });
        }
    }

    [AllowAnonymous]
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

    [Authorize(Roles = "Admin,Manager")]
    [HttpGet]
    [Route("create-from-ai")]
    public async Task<IActionResult> CreateFromAI()
    {
        var authors = await _authorRepository.GetAllAsync();
        ViewBag.AuthorNames = authors.Select(a => $"{a.FirstName} {a.LastName}").OrderBy(n => n).ToList();
        return View();
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost]
    [Route("create-from-ai")]
    public async Task<IActionResult> CreateFromAIPost([FromBody] AIPromptRequest request)
    {
        try
        {
            _logger.LogInformation("🤖 Primljen AI prompt za knjigu: {Prompt}", request.Prompt);

            if (string.IsNullOrWhiteSpace(request.Prompt))
                return BadRequest(new { error = "Prompt ne može biti prazan" });

            var bookData = await _aiService.ExtractBookFromPromptAsync(request.Prompt);

            var authors = await _authorRepository.GetAllAsync();
            var author = string.IsNullOrWhiteSpace(bookData.AuthorName)
                ? null
                : authors.FirstOrDefault(a =>
                    $"{a.FirstName} {a.LastName}".Contains(bookData.AuthorName, StringComparison.OrdinalIgnoreCase) ||
                    bookData.AuthorName.Contains(a.LastName, StringComparison.OrdinalIgnoreCase));

            if (author == null)
            {
                _logger.LogWarning("⚠️ Autor nije pronađen: {AuthorName}", bookData.AuthorName);
                return BadRequest(new
                {
                    error = $"Autor '{bookData.AuthorName}' nije pronađen",
                    suggestion = "Dostupni autori: " + string.Join(", ", authors.Select(a => $"{a.FirstName} {a.LastName}"))
                });
            }

            var publishers = await _publisherRepository.GetAllAsync();
            var publisher = publishers.FirstOrDefault();
            if (publisher == null)
            {
                _logger.LogWarning("⚠️ Nema dostupnih izdavača za kreiranje knjige");
                return BadRequest(new { error = "Nema dostupnih izdavača u sustavu" });
            }

            var book = new Book
            {
                Title = bookData.Title,
                Isbn = bookData.ISBN,
                Description = $"Knjiga kreirana putem AI upita: \"{request.Prompt}\"",
                PublishedOn = DateTime.UtcNow,
                PageCount = bookData.PageCount,
                Language = bookData.Language,
                Status = BookStatus.Available,
                AuthorId = author.Id,
                PublisherId = publisher.Id
            };

            await _bookRepository.CreateAsync(book);

            _logger.LogInformation("✅ Knjiga kreirana kroz AI: {BookTitle}, Autor: {AuthorName}", book.Title, bookData.AuthorName);

            return Ok(new
            {
                success = true,
                bookId = book.Id,
                message = $"✅ Knjiga '{book.Title}' uspješno kreirana",
                book = new
                {
                    title = book.Title,
                    author = $"{author.FirstName} {author.LastName}",
                    isbn = book.Isbn,
                    pageCount = book.PageCount
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Greška pri AI kreiranju knjige");
            return BadRequest(new { error = ex.Message });
        }
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

