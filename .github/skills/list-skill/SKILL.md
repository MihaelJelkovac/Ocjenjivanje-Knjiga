---
description: "Rad s List stranicama - Index view za prikaz kolekcija entiteta"
applyTo:
  - "**/Views/**/*.cshtml"
  - "**/Controllers/*Controller.cs"
---

# List Stranice (Index View) - Lab 3

Ovaj skill pružaja smjernice za kreiranju i unaprjeđenju Index (list) stranica za prikaz kolekcija podataka iz baze.

## Kada koristiti ovaj skill

- Trebate napraviti novu **Index view stranicu** za prikaz liste svih zapisa
- Trebate **poboljšati postojeću list stranicu** (dodati statistiku, search, filtering)
- Trebate dodati **responsive dizajn** (tablica za desktop, kartice za mobile)
- Trebate implementirati **empty state** (poruka kada nema podataka)

## Važne karakteristike List stranica

✅ **Index akcija** u kontroleru koji vraća sve zapise (async/await)
✅ **Model**: `IReadOnlyList<Entity>`
✅ **Breadcrumb navigacija** za bolje UX
✅ **Statistika** (broj zapisa, prosjeci, filtrirani rezultati)
✅ **Empty state** kada nema podataka
✅ **Responsive tablica** - table za desktop, kartice za mobile
✅ **Akcije** - linkovi na Details stranicu
✅ **Accessibility** - aria labels, semantic HTML

## Korak-po-korak: Osnovna List stranica

### Korak 1: Provjeri Controller Index akciju

```csharp
// Controllers/MyController.cs
[Route("[controller]")]
[Route("ruta")]
public class MyController : Controller
{
    private readonly IMyRepository _repository;

    public MyController(IMyRepository repository)
    {
        _repository = repository;
    }

    [Route("")]
    [Route("index")]
    public async Task<IActionResult> Index()
    {
        var items = await _repository.GetAllAsync();
        return View(items);  // Vraći listu
    }
}
```

### Korak 2: Kreiraj Index.cshtml u `Views/MyFolder/`

```html
@model IReadOnlyList<MyEntity>
@{
    ViewData["Title"] = "Moji entiteti";
}

<!-- Breadcrumb navigacija -->
<nav aria-label="breadcrumb" class="mb-3">
    <ol class="breadcrumb app-breadcrumb">
        <li class="breadcrumb-item">
            <a asp-controller="Home" asp-action="Index">Početna</a>
        </li>
        <li class="breadcrumb-item active" aria-current="page">
            Entiteti
        </li>
    </ol>
</nav>

<!-- Header sekcija -->
<section class="panel-card mb-4">
    <div class="panel-header">
        <div>
            <p class="section-label">Index</p>
            <h2>Popis entiteta</h2>
            <p class="panel-note mb-0">Brz pregled svih entiteta s mogućnošću detaljnog prikaza.</p>
        </div>
        <span class="panel-note">@Model.Count zapisa</span>
    </div>
</section>

<!-- Empty State (kada nema podataka) -->
@if (!Model.Any())
{
    <section class="panel-card" role="status" aria-live="polite">
        <h3>Nema dostupnih entiteta</h3>
        <p>Trenutno nema zapisa za prikaz.</p>
        <a class="btn btn-primary" asp-controller="Home" asp-action="Index">
            Povratak na početnu
        </a>
    </section>
}
else
{
    <!-- Tablica (vidljiva samo na desktop - d-lg-block) -->
    <section class="panel-card d-none d-lg-block" aria-label="Tablični prikaz">
        <div class="table-responsive">
            <table class="table app-table">
                <thead>
                    <tr>
                        <th scope="col">Naziv</th>
                        <th scope="col">Datum</th>
                        <th scope="col">Status</th>
                        <th scope="col">Akcija</th>
                    </tr>
                </thead>
                <tbody>
                    @foreach (var item in Model)
                    {
                        <tr>
                            <td>
                                <a asp-action="Details" asp-route-id="@item.Id">
                                    @item.Name
                                </a>
                            </td>
                            <td>@item.CreatedAt.ToString("dd.MM.yyyy.")</td>
                            <td><span class="status-badge">Aktivan</span></td>
                            <td>
                                <a class="app-cta-link" asp-action="Details" asp-route-id="@item.Id">
                                    Detalji
                                </a>
                            </td>
                        </tr>
                    }
                </tbody>
            </table>
        </div>
    </section>

    <!-- Kartice za mobile (vidljive samo na mobilnom - d-lg-none) -->
    <section class="mobile-cards d-lg-none" aria-label="Kartični prikaz">
        @foreach (var item in Model)
        {
            <article class="panel-card">
                <h3>@item.Name</h3>
                <p><strong>Datum:</strong> @item.CreatedAt.ToString("dd.MM.yyyy.")</p>
                <a class="btn btn-primary" asp-action="Details" asp-route-id="@item.Id">
                    Detalji
                </a>
            </article>
        }
    </section>
}
```

---

## Korak-po-korak: Napredna List stranica sa Statistikom

### Korak 1: Dodaj kalkulacije u view top

```csharp
@model IReadOnlyList<MyEntity>
@{
    ViewData["Title"] = "Entiteti";
    
    // Kalkulacije za statistiku
    var totalCount = Model.Count;
    var activeCount = Model.Count(x => x.Status == Status.Active);
    var inactiveCount = totalCount - activeCount;
    var averageRating = totalCount == 0 ? 0 : Model.Average(x => x.Rating);
}
```

### Korak 2: Prikaži statistiku

```html
<section class="panel-card stats-hero mb-4" aria-labelledby="stats-heading">
    <div class="panel-header">
        <div>
            <p class="section-label">Index</p>
            <h2 id="stats-heading">Katalog entiteta</h2>
        </div>
        <span class="panel-note">@totalCount zapisa</span>
    </div>

    <div class="stats-grid" role="list" aria-label="Sažetak statistike">
        <article class="stat-card" role="listitem">
            <span>Ukupno</span>
            <strong>@totalCount</strong>
        </article>
        <article class="stat-card" role="listitem">
            <span>Aktivnih</span>
            <strong>@activeCount</strong>
        </article>
        <article class="stat-card" role="listitem">
            <span>Neaktivnih</span>
            <strong>@inactiveCount</strong>
        </article>
        <article class="stat-card" role="listitem">
            <span>Prosječna ocjena</span>
            <strong>@averageRating.ToString("0.0")</strong>
        </article>
    </div>
</section>
```

---

## Best Practices

✓ **Koristi `IReadOnlyList<T>`** - sprječava slučajne izmjene
✓ **Uvijek dodaj empty state** - korisnik zna što se događa
✓ **Responsive design** - `d-none d-lg-block` za tablice, `d-lg-none` za kartice
✓ **Breadcrumb navigacija** - bolji UX i SEO
✓ **Aria labels** - za accessibility
✓ **Include relacije** - koristi `.Include()` u kontroleru kako izbegneš N+1 problema
✓ **Async/await** - uvijek koristi za database pozive
✓ **Link-ovi na Details** - `asp-action="Details" asp-route-id="@item.Id"`

---

## Primjer: Authors List stranica (POBOLJŠANA)

Ovo je primjer kako poboljšati postojeću Authors Index stranicu:

```html
@model IReadOnlyList<Author>
@{
    ViewData["Title"] = "Autori";
    var totalAuthors = Model.Count;
    var totalBooks = Model.Sum(a => a.Books.Count);
    var avgBooksPerAuthor = totalAuthors == 0 ? 0 : (decimal)totalBooks / totalAuthors;
}

<nav aria-label="breadcrumb" class="mb-3">
    <ol class="breadcrumb app-breadcrumb">
        <li class="breadcrumb-item"><a asp-controller="Home" asp-action="Index">Početna</a></li>
        <li class="breadcrumb-item active" aria-current="page">Autori</li>
    </ol>
</nav>

<section class="panel-card authors-hero mb-4" aria-labelledby="authors-heading">
    <div class="panel-header">
        <div>
            <p class="section-label">Index</p>
            <h2 id="authors-heading">Katalog autora</h2>
            <p class="panel-note mb-0">Pregledaj sve autore, njihovu nacionalnost i broj objavljenih knjiga.</p>
        </div>
        <span class="panel-note">@totalAuthors autora</span>
    </div>

    <div class="stats-grid" role="list" aria-label="Sažetak autora">
        <article class="stat-card" role="listitem">
            <span>Ukupno autora</span>
            <strong>@totalAuthors</strong>
        </article>
        <article class="stat-card" role="listitem">
            <span>Knjiga</span>
            <strong>@totalBooks</strong>
        </article>
        <article class="stat-card" role="listitem">
            <span>Prosječno po autoru</span>
            <strong>@avgBooksPerAuthor.ToString("0.0")</strong>
        </article>
    </div>
</section>

@if (!Model.Any())
{
    <section class="panel-card" role="status" aria-live="polite">
        <h3>Nema dostupnih autora</h3>
        <p>Trenutno nema zapisa za prikaz. Vrati se na početnu stranicu.</p>
        <a class="btn btn-primary" asp-controller="Home" asp-action="Index">Povratak na početnu</a>
    </section>
}
else
{
    <section class="panel-card authors-table d-none d-lg-block" aria-label="Tablica autora">
        <div class="table-responsive">
            <table class="table app-table">
                <thead>
                    <tr>
                        <th scope="col">Ime</th>
                        <th scope="col">Nacionalnost</th>
                        <th scope="col">Rođen</th>
                        <th scope="col">Knjige</th>
                        <th scope="col">Website</th>
                        <th scope="col">Akcija</th>
                    </tr>
                </thead>
                <tbody>
                    @foreach (var author in Model)
                    {
                        <tr>
                            <td>
                                <a asp-action="Details" asp-route-id="@author.Id">
                                    @author.FirstName @author.LastName
                                </a>
                            </td>
                            <td>@author.Nationality</td>
                            <td>@author.BirthDate.ToString("dd.MM.yyyy.")</td>
                            <td>
                                <span class="badge badge-info">@author.Books.Count</span>
                            </td>
                            <td>
                                @if (!string.IsNullOrEmpty(author.Website))
                                {
                                    <a href="@author.Website" target="_blank" rel="noopener">
                                        Posjet
                                    </a>
                                }
                                else
                                {
                                    <span>-</span>
                                }
                            </td>
                            <td>
                                <a class="app-cta-link" asp-action="Details" asp-route-id="@author.Id">
                                    Profil
                                </a>
                            </td>
                        </tr>
                    }
                </tbody>
            </table>
        </div>
    </section>

    <section class="authors-mobile d-lg-none" aria-label="Kartični prikaz autora">
        @foreach (var author in Model)
        {
            <article class="panel-card author-card">
                <h3>@author.FirstName @author.LastName</h3>
                <p><strong>Nacionalnost:</strong> @author.Nationality</p>
                <p><strong>Rođen:</strong> @author.BirthDate.ToString("dd.MM.yyyy.")</p>
                <p><strong>Knjige:</strong> <span class="badge badge-info">@author.Books.Count</span></p>
                @if (!string.IsNullOrEmpty(author.Website))
                {
                    <p>
                        <a href="@author.Website" target="_blank" rel="noopener">
                            Website →
                        </a>
                    </p>
                }
                <a class="btn btn-primary" asp-action="Details" asp-route-id="@author.Id">
                    Profil autora
                </a>
            </article>
        }
    </section>
}
```

---

## Best Practices - Kontroler

```csharp
[Route("[controller]")]
[Route("autori")]
public class AuthorsController : Controller
{
    private readonly IAuthorRepository _authorRepository;

    public AuthorsController(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    [Route("")]
    [Route("index")]
    public async Task<IActionResult> Index()
    {
        // Dohvati sve autore + njihove knjige (Include)
        // Ovo sprječava N+1 problem
        var authors = await _authorRepository.GetAllAsync();
        
        return View(authors);
    }
}
```

---

## Često postavljana pitanja (FAQ)

### P: Kako dodati filter po status-u?

```csharp
[Route("")]
[Route("index")]
public async Task<IActionResult> Index(string filter = "all")
{
    var authors = await _authorRepository.GetAllAsync();
    
    if (filter != "all")
    {
        authors = authors.Where(a => a.Status == filter).ToList();
    }
    
    return View(authors);
}
```

### P: Kako dodati search?

```html
<form asp-action="Index" method="get" class="search-form">
    <input type="text" name="search" placeholder="Pretraži autore..." 
           class="form-control" value="@ViewData["search"]">
    <button type="submit" class="btn btn-primary">Pretraži</button>
</form>
```

### P: Što ako je lista jako velika?

- Implementiraj **pagination** (prvu stranicu, sljedeću, prethodnu)
- Koristi `.Skip()` i `.Take()` u LINQ upitu
- Razmotrი **virtualizaciju** na frontend-u

---

## Datoteke važne za List stranice

| Datoteka | Uloga |
|----------|-------|
| `Controllers/MyController.cs` | Index akcija - vraća sve zapise |
| `Views/MyFolder/Index.cshtml` | Prikaz liste svih zapisa |
| `Services/IMyRepository.cs` | Interfejs za dohvat podataka |
| `Models/MyEntity.cs` | Model klasa s relacijama |

---

**Verzija**: 1.0  
**Zadnja ažurenja**: 2026-05-07  
**Autor**: GitHub Copilot
