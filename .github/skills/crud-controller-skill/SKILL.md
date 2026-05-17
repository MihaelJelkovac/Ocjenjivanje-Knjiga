# CRUD Controller Skill — Lab-4 Controller Actions

**Svrha**: Generirati sve CRUD akcije (Create, Edit, Delete) u kontrolerima prema Lab4.md zahtjevima s validacijom, TryUpdateModel, soft delete i popunjavanjem ViewBag za dropdowne.

## Input Parametri

- `entityName` — Naziv entiteta (Author, Book, Genre, Publisher, Review, User)
- `controllerName` — Naziv kontrolera (AuthorsController, BooksController, itd.)
- `repositoryInterface` — Interfejs repozitorija (IAuthorRepository, IBookRepository, itd.)
- `foreignKeys` — Lista FK polja ako postoje (npr. [{ field: "AuthorId", repoInterface: "IAuthorRepository", listProperty: "Authors" }])

## Output — Sve Akcije

### 1. Create — GET (Prikaz Forme)

```csharp
[HttpGet]
[Route("create")]
public async Task<IActionResult> Create()
{
    // Popuni ViewBag s opcijama za sve dropdowne
    await PopulateDropdownsAsync();
    
    return View(new T());
}
```

### 2. Create — POST (Spremanje)

```csharp
[HttpPost]
[Route("create")]
public async Task<IActionResult> Create(T model)
{
    if (!ModelState.IsValid)
    {
        await PopulateDropdownsAsync();
        return View(model);
    }
    
    try
    {
        var id = await _repository.CreateAsync(model);
        return RedirectToAction(nameof(Index));
    }
    catch (Exception ex)
    {
        ModelState.AddModelError(string.Empty, "Greška pri spremanju: " + ex.Message);
        await PopulateDropdownsAsync();
        return View(model);
    }
}
```

### 3. Edit — GET (Prikaz Forme za Edit)

```csharp
[HttpGet]
[Route("edit/{id:int}")]
[ActionName("Edit")]
public async Task<IActionResult> EditGet(int id)
{
    var entity = await _repository.GetByIdAsync(id);
    if (entity == null)
    {
        return NotFound();
    }
    
    await PopulateDropdownsAsync(entity); // Prosljeđi entitet da zna koji je trenutno odabran
    
    return View("Edit", entity);
}
```

### 4. Edit — POST (Ažuriranje)

```csharp
[HttpPost]
[Route("edit/{id:int}")]
[ActionName("Edit")]
public async Task<IActionResult> EditPost(int id)
{
    var entity = await _repository.GetByIdAsync(id);
    if (entity == null)
    {
        return NotFound();
    }
    
    // Koristi TryUpdateModel za sigurnost
    var updateOk = await TryUpdateModelAsync(entity);
    
    if (!updateOk || !ModelState.IsValid)
    {
        await PopulateDropdownsAsync(entity);
        return View("Edit", entity);
    }
    
    try
    {
        var success = await _repository.UpdateAsync(entity);
        if (!success)
        {
            throw new Exception("Neuspješno ažuriranje u bazi");
        }
        
        return RedirectToAction(nameof(Index));
    }
    catch (Exception ex)
    {
        ModelState.AddModelError(string.Empty, "Greška pri ažuriranju: " + ex.Message);
        await PopulateDropdownsAsync(entity);
        return View("Edit", entity);
    }
}
```

### 5. Delete — POST (Soft Delete)

```csharp
[HttpPost]
[Route("delete/{id:int}")]
public async Task<IActionResult> Delete(int id)
{
    try
    {
        var success = await _repository.DeleteAsync(id);
        
        if (!success)
        {
            return Json(new { success = false, message = "Entitet nije pronađen" });
        }
        
        return Json(new { success = true, message = "Entitet je uspješno obrisan" });
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = "Greška pri brisanju: " + ex.Message });
    }
}
```

### 6. PopulateDropdownsAsync — Helper Metoda

```csharp
private async Task PopulateDropdownsAsync(T? entity = null)
{
    // Primjer za Book:
    var authors = await _authorRepository.GetAllAsync();
    ViewBag.Authors = authors.Select(a => new SelectListItem 
    { 
        Value = a.Id.ToString(), 
        Text = $"{a.FirstName} {a.LastName}",
        Selected = entity != null && entity.AuthorId == a.Id
    }).ToList();
    
    var publishers = await _publisherRepository.GetAllAsync();
    ViewBag.Publishers = publishers.Select(p => new SelectListItem 
    { 
        Value = p.Id.ToString(), 
        Text = p.Name,
        Selected = entity != null && entity.PublisherId == p.Id
    }).ToList();
    
    var genres = await _genreRepository.GetAllAsync();
    ViewBag.Genres = genres.Select(g => new SelectListItem 
    { 
        Value = g.Id.ToString(), 
        Text = g.Name
    }).ToList();
    
    // Za statičke dropdowne:
    ViewBag.Statuses = Enum.GetValues(typeof(BookStatus))
        .Cast<BookStatus>()
        .Select(s => new SelectListItem 
        { 
            Value = s.ToString(), 
            Text = s.ToString(),
            Selected = entity != null && entity.Status == s
        })
        .ToList();
}
```

## Primjer Kompletnog AuthorsController

```csharp
using Lab3.Models;
using Lab3.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab3.Controllers;

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
}
```

## Primjer Kompletnog BooksController (s FK Dropdownima)

```csharp
using Lab3.Models;
using Lab3.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab3.Controllers;

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
```

## Važne Napomene

1. **TryUpdateModelAsync**: MORA biti korištena u EditPost akciji za sigurnost
2. **ActionName atribut**: EditGet i EditPost trebaju `[ActionName("Edit")]` jer imaju istu URL
3. **ModelState.IsValid**: MORA biti provjereno prije spremanja
4. **Soft Delete**: DeleteAsync postavlja DeletedAt, ne briše zapis
5. **PopulateDropdownsAsync**: Trebam je pozvati i u Create GET i POST (ako validacija ne prođe)
6. **ViewBag**: Koristi se za prosljeđivanje opcija u view

## Verifikacija

- [ ] Create (GET i POST) akcije postoje
- [ ] Edit (GET i POST) akcije postoje s ActionName atributom
- [ ] Delete (POST) akcija vraća JSON s success/message
- [ ] ModelState.IsValid je provjereno
- [ ] TryUpdateModelAsync je korištena u EditPost
- [ ] PopulateDropdownsAsync je pozvan na svim potrebnim mjestima
- [ ] Kod kompajlira bez grešaka
