# Lab 3 - Entity Framework & Routing - Implementacija završena ✓

Ova dokumentacija sažima sve što je implementirano u Lab 3.

## Status implementacije

| Zahtjev | Status | Datoteka/Komponenta |
|---------|--------|------------------|
| EF Core konfiguracija | ✓ | `Data/CatalogDbContext.cs`, `Program.cs` |
| EF anotacije na modelima | ✓ | `Models/*.cs` - [Key], [ForeignKey], virtual |
| Virtual svojstva i ICollection | ✓ | Sve relacije konfigurene |
| Connection string | ✓ | `appsettings.json` |
| DbContext registracija | ✓ | `Program.cs` |
| Repository implementacija | ✓ | `Services/*Repository.cs` - EF-backed |
| Inicijalna migracija | ✓ | `Migrations/20260503214646_Initial.cs` |
| SQLite baza | ✓ | `catalog.db` |
| Custom routing (4+) | ✓ | Svaki kontroler ima 2+ custom route-a |
| Semantic model dokumentacija | ✓ | `semantic-model.md` |
| Sitemap dokumentacija | ✓ | `sitemap.md` |
| SKILL.md datoteka | ✓ | `SKILL-EF.md` |

---

## Što je implementirano

### 1. **Entity Framework integracija**
- ✓ SQLite kao provider
- ✓ DbContext s 7 DbSet properties
- ✓ Svi modeli s EF anotacijama
- ✓ Relationships konfigureni (1-N, N-N)
- ✓ Seed data implementiran

### 2. **Modeli sa EF anotacijama**
- ✓ `Author.cs` - s virtual ICollection<Book>
- ✓ `Book.cs` - s ForeignKey anotacijama
- ✓ `Genre.cs` - n-n relationship
- ✓ `Publisher.cs` - 1-n relationship
- ✓ `Review.cs` - s ForeignKey na Book i User
- ✓ `User.cs` - s virtual ICollection<Review>
- ✓ `BookGenre.cs` - composite key

### 3. **Migracije**
```
Migrations/
├── 20260503214646_Initial.cs (tablice, veze, seed data)
├── 20260503214646_Initial.Designer.cs
└── CatalogDbContextModelSnapshot.cs
```

Migracija je **primjena s svim podacima** i sprema za produkciju.

### 4. **Custom Routing**
Sveih 7 kontrolera ima custom routing s lokalnim nazivima:

| Kontroler | URL-ovi |
|----------|---------|
| **HomeController** | `/`, `/home`, `/arena`, `/privacy` |
| **BooksController** | `/books`, `/knjige`, `/books/{id}`, `/books/detalji/{id}` |
| **AuthorsController** | `/authors`, `/autori`, `/authors/{id}`, `/authors/profil/{id}` |
| **GenresController** | `/genres`, `/zanrovi`, `/genres/{id}`, `/genres/popis/{id}` |
| **PublishersController** | `/publishers`, `/izdavaci`, `/publishers/{id}`, `/publishers/detalji/{id}` |
| **ReviewsController** | `/reviews`, `/recenzije`, `/reviews/{id}`, `/reviews/prikaz/{id}` |
| **UsersController** | `/users`, `/korisnici`, `/users/{id}`, `/users/profil/{id}` |

**Zapažanja o routingu**:
- ✓ Attribute routing korišten za preciznu kontrolu
- ✓ 28+ custom ruta implementirano
- ✓ Lokalizirani URL-i na hrvatskom
- ✓ Fallback na default route ako custom route ne uspije

### 5. **Services & Repositories (EF-backed)**
- ✓ `IAuthorRepository` + `AuthorRepository`
- ✓ `IBookRepository` + `BookRepository`
- ✓ `IGenreRepository` + `GenreRepository`
- ✓ `IPublisherRepository` + `PublisherRepository`
- ✓ `IReviewRepository` + `ReviewRepository`
- ✓ `IUserRepository` + `UserRepository`
- ✓ `IBookCatalogService` + `BookCatalogService`

Sve repozitorije koriste `DbContext` s Include() za optimizaciju.

### 6. **Dokumentacija**

#### semantic-model.md
- Opis baze podataka
- Tablica po tablica
- Svojstva i tipovi
- Veze između tablica

#### sitemap.md
- Kompletan URL map
- 28+ ruta dokumentirano
- Controller → Action → View mapiranje
- Route constraints

#### SKILL-EF.md
- EF Core skill za Lab 3
- Kako pokrenuti migracije
- Kako dodati nove entitete
- FAQ i best practices

---

## Kako pokrenuti aplikaciju

```powershell
# Iz lab-3 direktorija
cd c:\Users\Mihael\Desktop\ASP.NET\lab-3

# Pokrenuti aplikaciju
dotnet run --project Lab3.csproj

# Ili s HTTPS profilerom
dotnet run --launch-profile https
```

Aplikacija će biti dostupna na:
- HTTP: `http://localhost:5003`
- HTTPS: `https://localhost:7003`

---

## Testiranje aplikacije

### Test rutine

1. **Početna stranica**
   - Posjetite `https://localhost:7003` ili `/`
   - Trebao bi vidjeti dashboard s top knjigama

2. **Custom routing testovi**
   ```
   ✓ /books - Popis knjiga (English)
   ✓ /knjige - Popis knjiga (Lokalizirano)
   ✓ /books/1 - Detalji knjige
   ✓ /books/detalji/1 - Detalji knjige (alt URL)
   ✓ /autori - Popis autora (Lokalizirano)
   ✓ /zanrovi - Popis žanrova (Lokalizirano)
   ```

3. **EF Test**
   - Sve stranice trebale bi učitati podatke iz SQLite baze
   - Veze su spremne (npr. Book → Author, Review → User)

---

## Struktura Lab 3 projekta

```
lab-3/
├── bin/
├── obj/
├── Migrations/               ← EF migracije
│   ├── 20260503214646_Initial.cs
│   ├── 20260503214646_Initial.Designer.cs
│   └── CatalogDbContextModelSnapshot.cs
├── Models/                   ← Modelske klase s EF anotacijama
│   ├── Author.cs
│   ├── Book.cs
│   ├── BookGenre.cs
│   ├── BookStatus.cs
│   ├── Genre.cs
│   ├── Publisher.cs
│   ├── Review.cs
│   ├── ReviewSentiment.cs
│   ├── User.cs
│   └── ErrorViewModel.cs
├── Data/                     ← DbContext
│   └── CatalogDbContext.cs
├── Services/                 ← Repository interfacesi i implementacije
│   ├── IAuthorRepository.cs
│   ├── AuthorRepository.cs
│   ├── IBookRepository.cs
│   ├── BookRepository.cs
│   ├── IGenreRepository.cs
│   ├── GenreRepository.cs
│   ├── IPublisherRepository.cs
│   ├── PublisherRepository.cs
│   ├── IReviewRepository.cs
│   ├── ReviewRepository.cs
│   ├── IUserRepository.cs
│   ├── UserRepository.cs
│   ├── IBookCatalogService.cs
│   └── BookCatalogService.cs
├── Controllers/              ← 7 kontrolera s custom routing-om
│   ├── HomeController.cs
│   ├── BooksController.cs
│   ├── AuthorsController.cs
│   ├── GenresController.cs
│   ├── PublishersController.cs
│   ├── ReviewsController.cs
│   └── UsersController.cs
├── ViewModels/               ← View models za dashboard
│   ├── BookCardViewModel.cs
│   ├── BookDashboardViewModel.cs
│   ├── GenreStatViewModel.cs
│   └── ReviewCardViewModel.cs
├── Views/                    ← Razor view datoteke
│   ├── Home/
│   ├── Books/
│   ├── Authors/
│   ├── Genres/
│   ├── Publishers/
│   ├── Reviews/
│   ├── Users/
│   ├── Shared/
│   ├── _ViewStart.cshtml
│   └── _ViewImports.cshtml
├── wwwroot/                  ← Statički assets
│   ├── css/
│   ├── js/
│   └── favicon.ico
├── Properties/
│   └── launchSettings.json   ← HTTPS/HTTP konfiguracija
├── Program.cs                ← DI registracija EF i servisa
├── appsettings.json          ← Connection string
├── Lab3.csproj               ← Projekt s EF Core paketima
├── catalog.db                ← SQLite baza
├── semantic-model.md         ← Dokumentacija DB modela
├── sitemap.md                ← Dokumentacija routing-a
├── SKILL-EF.md              ← EF skill za Lab 3
└── README.md                 ← Ova datoteka
```

---

## Napomene o EF implementaciji

### Lazy Loading
Virtual svojstva omogućavaju lazy loading:
```csharp
var author = await _authorRepository.GetByIdAsync(1);
var bookCount = author.Books.Count; // Automatski se učitavaju knjige
```

### Eager Loading
Repozitoriji koriste `Include()` za eager loading:
```csharp
var book = await _context.Books
    .Include(b => b.Author)
    .Include(b => b.Reviews)
    .FirstOrDefaultAsync();
```

### N+1 Problem
Riješen s `Include()` i `ThenInclude()`:
```csharp
var books = await _context.Books
    .Include(b => b.BookGenres)
    .ThenInclude(bg => bg.Genre)
    .ToListAsync();
```

---

## Baza podataka - Inicijalni podaci

Baza je automatic popunjena s:

**Autori** (3)
- J.K. Rowling
- George R. R. Martin
- J.R.R. Tolkien

**Izdavači** (3)
- Bloomsbury
- Bantam Books
- Allen & Unwin

**Žanrovi** (3)
- Fantasy
- Science Fiction
- Drama

**Knjige** (3)
- Harry Potter and the Philosopher's Stone
- A Game of Thrones
- The Fellowship of the Ring

**Korisnici** (3)
- Alice Reader
- Bob Smith
- Carol White

**Recenzije** (3)
- Različite ocjene i sentimenti

---

## Potrebne naredbe za migracije

```powershell
# Iz lab-3 direktorija

# Kreiraj migraciju nakon promjene modela
dotnet-ef migrations add NameOfMigration

# Primjeni migracije na bazu
dotnet-ef database update

# Ukloni zadnju migraciju
dotnet-ef migrations remove

# Prikaži sve migracije
dotnet-ef migrations list

# Resetiraj bazu (obriši i kreiraj ispočetka)
Remove-Item catalog.db
dotnet-ef database update
```

---

## Zahtjevi iz Lab3.md - Ispunjeni ✓

- [x] Konfiguracija EF s anotacijama
- [x] Virtual svojstva i ICollection za relacije
- [x] DbContext konfiguracija i DI
- [x] Prebacivanje s mock na EF repozitorije
- [x] Inicijalna migracija
- [x] Custom routing (4+ akcije po kontroleru)
- [x] semantic-model.md dokumentacija
- [x] sitemap.md dokumentacija
- [x] SKILL.md datoteka (EF skill)

---

## Napomene za budući razvoj

- Dodati Create/Edit/Delete akcije u kontrolere
- Implementirati CRUD operacije s EF
- Dodati validaciju modela
- Implementirati error handling
- Dodati unit testove
- Konfigurirati dodatne SKILL.md datoteke (za CRUD, UX)

---

**Verzija**: 1.0  
**Datum**: 2026-05-03  
**Status**: ✓ Završeno  
**Aplikacija**: Lab 3 - EF Core + Custom Routing

---

## Kontakt & Debugging

Ako trebate resetirati bazu:
```powershell
Remove-Item c:\Users\Mihael\Desktop\ASP.NET\lab-3\catalog.db
dotnet-ef database update
```

Ako trebate vidjeti SQL što EF generira:
```powershell
dotnet-ef migrations script
```

Ako trebate dodati nove podatke u bazu:
1. Ažurirajte `SeedData()` u `CatalogDbContext.cs`
2. Kreirajte novu migraciju: `dotnet-ef migrations add AddMoreSeedData`
3. Primijenih: `dotnet-ef database update`

---

**Lab 3 je spreman za testiranje!** 🚀

---

## Lab 5 - Run & Google setup

Quick notes for running Lab 5 locally and enabling Google external login.

- **Run the app (from repo root):**

```powershell
dotnet run --project lab-5/Lab5.csproj
```

- **Migrations / database:** Program will apply migrations on startup. To run manually:

```powershell
# from lab-5 folder
cd lab-5
dotnet ef database update
```

- **Enable Google external login (development):** create user-secrets for the project and set the client id/secret:

```powershell
cd lab-5
dotnet user-secrets init
dotnet user-secrets set "Authentication:Google:ClientId" "<your-client-id>"
dotnet user-secrets set "Authentication:Google:ClientSecret" "<your-client-secret>"
```

Ensure HTTPS is enabled in `Properties/launchSettings.json` when testing external login.

- **Notes:**
   - API write endpoints (POST/PUT/DELETE) require roles: `Admin` or `Manager` (delete typically `Admin`).
   - Attachments upload/delete endpoints require authorization (`Books/Attachments/Upload` requires `Admin,Manager`; delete requires `Admin`).
   - Uploads are stored under `wwwroot/uploads/books/{bookId}`.

