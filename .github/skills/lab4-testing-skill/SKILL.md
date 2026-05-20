---
description: "Testiranje Lab-4 projekta - CRUD operacije, validacija, autocomplete, datumska kontrola"
applyTo:
  - "**/Controllers/*Controller.cs"
  - "**/Services/*Repository.cs"
  - "**/Models/*.cs"
---

# Lab 4 - Strategija Testiranja

Ovaj skill pružaja smjernice za testiranje Lab 4 projekta. Prema zahtjevima vježbe trebate testirati:

✅ **CRUD operacije** - Create, Read, Update, Delete  
✅ **Validacija** - client-side + server-side  
✅ **Autocomplete dropdown** - AJAX pretraga  
✅ **Soft delete** - DeletedAt polje umjesto fizičkog brisanja  
✅ **Datumska kontrola** - format ovisno o kulturi (hr/en)  
✅ **JavaScript animacije** - napredno korištenje JavaScripta  

---

## Što Testirati - Test Strategija

### 1. **CRUD Operacije**

| Operacija | Test Scenarij | Očekivani Rezultat |
|-----------|--------------|-------------------|
| **Create** | Kreiraj novu entitetu s valjanim podacima | Entitet se pojavljuje u bazi |
| | Kreiraj s nevaljanim podacima | Validacijska greška, bez spremanja |
| | Kreiraj s praznom obaveznom poljem | Server validacija odbija |
| **Read** | Učitaj listu entiteta | Sve entitete (gdje je DeletedAt = null) |
| | Učitaj detalje jedne entitete | Prikazane sve informacije |
| **Update** | Ažuriraj postojeću entitetu | Promjene se spravljaju u bazu |
| | Ažuriraj s nevaljanim podacima | Validacijska greška |
| **Delete** | Obriši entitetu | DeletedAt se postavlja (soft delete) |
| | Obrisana entiteta ne pojavljuje se u listama | Filter na DeletedAt = null radi |

### 2. **Validacija**

- **Server-side:** Model.IsValid u controlleru
- **Client-side:** HTML5 atributi + jQuery validacija
- **Testiranje:** POST zahtjev s nevaljanim podacima mora vratiti 400 ili ponovno prikazati view

### 3. **Autocomplete Dropdown**

- Endpoint koji vraća JSON rezultate pretrage
- AJAX poziv s parametrom `query`
- Vraćeni rezultati formatiraju se u dropdownu

### 4. **Soft Delete**

```csharp
// Umjesto brisanja, postavi DeletedAt
entity.DeletedAt = DateTime.UtcNow;
await _dbContext.SaveChangesAsync();

// Kod čitanja filtrira obrisane
var items = await _dbContext.Set<MyEntity>()
    .Where(x => x.DeletedAt == null)
    .ToListAsync();
```

---

## Primjer: Test CRUD Operacije za Book Entitet

### Preduvjet

Trebate xUnit + testni DbContext s InMemory bazom podataka.

### Instalacija

```powershell
cd c:\Users\Mihael\Desktop\ASP.NET\lab-4

# Dodaj test pakete (ako nedostaju)
dotnet add package Microsoft.EntityFrameworkCore.InMemory
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
```

---

## Test Primjer - BookRepository CRUD Test

Kreirajte datoteku `Tests/BookRepositoryTests.cs`:

```csharp
using Xunit;
using Lab4.Models;
using Lab4.Services;
using Lab4.Data;
using Microsoft.EntityFrameworkCore;

namespace Lab4.Tests
{
    /// <summary>
    /// Testira sve CRUD operacije za Books entitet
    /// </summary>
    public class BookRepositoryTests
    {
        // Metoda koja kreira testni DbContext s InMemory bazom
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())  // Jedinstvena baza za svaki test
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        /// <summary>
        /// Test: Kreiraj novu knjižu i provjeri da se pojavljuje u bazi
        /// 
        /// Scenarij: Administrator dodaje novu knjižu
        /// 1. Kreira novu Book entitetu s valjanim podacima
        /// 2. Sprema ju u bazu kroz repository
        /// 3. Provjerava da se knjižu može učitati po ID-u
        /// 4. Provjerava da su svi podaci točni
        /// </summary>
        [Fact]
        public async Task CreateBook_WithValidData_ShouldSaveToDatabase()
        {
            // ARRANGE - Pripremite testni scenarij
            var context = GetInMemoryDbContext();
            var repository = new BookRepository(context);

            var newBook = new Book
            {
                Title = "Gospodar Prstenova",
                Isbn = "978-0547928227",
                PublicationDate = new DateTime(1954, 7, 29),
                Pages = 569,
                AuthorId = 1,
                PublisherId = 1
            };

            // ACT - Izvedite akciju koju testzirate
            await repository.CreateAsync(newBook);
            await context.SaveChangesAsync();

            // ASSERT - Provjerite rezultat
            var savedBook = await context.Books
                .FirstOrDefaultAsync(b => b.Title == "Gospodar Prstenova");

            Assert.NotNull(savedBook);
            Assert.Equal("978-0547928227", savedBook.Isbn);
            Assert.Equal(569, savedBook.Pages);
            Assert.Null(savedBook.DeletedAt);  // Ne smije biti obrisana
        }

        /// <summary>
        /// Test: Ažuriraj postojeću knjižu i provjeri da se promjene spravljaju
        /// 
        /// Scenarij: Administrator mijenja broj stranica knjižu
        /// 1. Učita postojeću knjižu iz baze
        /// 2. Mijenja Pages svojstvo
        /// 3. Sprema promjene
        /// 4. Provjerava da se promjena vidi u bazi
        /// </summary>
        [Fact]
        public async Task UpdateBook_WithNewPageCount_ShouldUpdateDatabase()
        {
            // ARRANGE
            var context = GetInMemoryDbContext();
            var repository = new BookRepository(context);

            // Dodaj test knjižu
            var book = new Book
            {
                Title = "Hobbit",
                Isbn = "978-0547928186",
                Pages = 310,
                PublicationDate = new DateTime(1937, 9, 21)
            };
            context.Books.Add(book);
            await context.SaveChangesAsync();

            var bookId = book.Id;

            // ACT - Ažuriraj broj stranica
            var bookToUpdate = await context.Books.FindAsync(bookId);
            bookToUpdate.Pages = 320;  // Promijenjena vrijednost
            context.Books.Update(bookToUpdate);
            await context.SaveChangesAsync();

            // ASSERT - Provjeri da se promjena vidi
            var updatedBook = await context.Books.FindAsync(bookId);
            Assert.Equal(320, updatedBook.Pages);
        }

        /// <summary>
        /// Test: Obriši knjižu (soft delete) i provjeri da DeletedAt bude postavljen
        /// 
        /// Scenarij: Administrator briše knjižu iz kataloga
        /// 1. Učita knjižu iz baze
        /// 2. Postavi DeletedAt na trenutno vrijeme
        /// 3. Sprema promjene
        /// 4. Provjerava da obrisana knjižu ne pojavljuje se u listama
        /// </summary>
        [Fact]
        public async Task DeleteBook_SoftDelete_ShouldSetDeletedAtNotNull()
        {
            // ARRANGE
            var context = GetInMemoryDbContext();
            var repository = new BookRepository(context);

            // Dodaj test knjižu
            var book = new Book
            {
                Title = "1984",
                Isbn = "978-0451524935",
                Pages = 328
            };
            context.Books.Add(book);
            await context.SaveChangesAsync();

            var bookId = book.Id;

            // ACT - Soft delete (postavi DeletedAt)
            var bookToDelete = await context.Books.FindAsync(bookId);
            bookToDelete.DeletedAt = DateTime.UtcNow;
            context.Books.Update(bookToDelete);
            await context.SaveChangesAsync();

            // ASSERT - Provjeri da se obrisana knjižu ne pojavljuje
            var activeBooks = await context.Books
                .Where(b => b.DeletedAt == null)
                .ToListAsync();

            Assert.DoesNotContain(activeBooks, b => b.Id == bookId);

            // Provjeri da DeletedAt nije null
            var deletedBook = await context.Books.FindAsync(bookId);
            Assert.NotNull(deletedBook.DeletedAt);
        }

        /// <summary>
        /// Test: Validacija obaveznog polja (Title)
        /// 
        /// Scenarij: Korisnik pokušava kreiranu knjižu bez naslova
        /// 1. Kreira entitetu bez Title svojstva
        /// 2. Validacija bi trebala detektirati grešku
        /// 3. Podaci se ne spravljaju u bazu
        /// </summary>
        [Fact]
        public async Task CreateBook_WithoutTitle_ShouldFailValidation()
        {
            // ARRANGE
            var context = GetInMemoryDbContext();
            var book = new Book
            {
                Title = null,  // Nedostaje obavezno polje
                Isbn = "978-0451524935",
                Pages = 328
            };

            // ACT & ASSERT - Validation će biti provjeravana na model level
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(book);
            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(book, validationContext, results, true);

            Assert.False(isValid);
            Assert.NotEmpty(results);
        }
    }
}
```

---

## Kako Pokrenuti Testove

### 1. **Instalacija Test Framework-a** (ako nedostaje)

```powershell
cd c:\Users\Mihael\Desktop\ASP.NET\lab-4

# xUnit je obično već instaliran
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
dotnet add package Microsoft.EntityFrameworkCore.InMemory
```

### 2. **Pokretanje Testova iz Terminala**

```powershell
cd c:\Users\Mihael\Desktop\ASP.NET\lab-4

# Pokreni sve testove
dotnet test

# Pokreni samo određeni test
dotnet test --filter "BookRepositoryTests"

# Pokreni s verbozim outputom
dotnet test -v d
```

### 3. **Pokretanje Testova iz VS Codea**

- Otvori **Test Explorer** (`Ctrl+Shift+@` ili preko Command Palette)
- Desni klik na test → **Run Test**
- Ili kliknite **Run All** za sve testove

---

## Što Testira Gornji Primjer?

| Test | Što Provjerava | Razlog |
|------|----------------|--------|
| `CreateBook_WithValidData` | CRUD Create operacija | Može li se novaJest knjižu sprema u bazu? |
| `UpdateBook_WithNewPageCount` | CRUD Update operacija | Može li se postojeća knjižu ažurirati? |
| `DeleteBook_SoftDelete` | Soft Delete (DeletedAt) | Je li DeletedAt postavljen, a knjižu logički obrisana? |
| `CreateBook_WithoutTitle` | Server-side Validacija | Sprječava li validacija unos nevaljanog Title-a? |

---

## Dodatni Test Scenariji (Napredni)

### Test Autocomplete Endpointa

```csharp
[Fact]
public async Task SearchBooks_WithQuery_ShouldReturnFilteredResults()
{
    // ARRANGE
    var context = GetInMemoryDbContext();
    context.Books.Add(new Book { Title = "Hobbit" });
    context.Books.Add(new Book { Title = "1984" });
    context.Books.Add(new Book { Title = "Hobbit Returns" });
    await context.SaveChangesAsync();

    // ACT - Pretraga Books gdje Title sadrži "Hobbit"
    var results = context.Books
        .Where(b => b.Title.Contains("Hobbit") && b.DeletedAt == null)
        .Take(10)
        .ToList();

    // ASSERT
    Assert.Equal(2, results.Count);
    Assert.All(results, b => Assert.Contains("Hobbit", b.Title));
}
```

### Test Validacije Datumske Kontrole

```csharp
[Theory]
[InlineData("2024-01-01")]
[InlineData("1900-01-01")]
[InlineData("2099-12-31")]
public void PublicationDate_WithValidDates_ShouldParse(string dateString)
{
    // Test da parser podržava različite datumske formate
    var parsed = DateTime.Parse(dateString);
    Assert.NotEqual(default, parsed);
}
```

---

## Checklist - Što Testirati u Lab 4

- [ ] Kreiraj novu entitetu (Create)
- [ ] Učitaj listu entiteta (Read)
- [ ] Ažuriraj entitetu (Update)
- [ ] Obriši entitetu s soft delete (Delete)
- [ ] Validacija obaveznog polja (Server-side)
- [ ] Autocomplete pretraga vraća filtrirane rezultate
- [ ] DeletedAt filter - obrisane ne pojavljuju se
- [ ] Datumska kontrola - format ovisno o kulturi
- [ ] AJAX dropdown - asinkrona pretraga radi

---

## Savjet: Testirajte Prvo Jedno Entitete Potpuno

Bolje je imati 5 dobrih testova za jedan entitet nego 20 loših testova za sve. Nakon što su testovi za Book entitet savršeni, lako je kopirati isti pattern za Author, Genre, Publisher itd.

```
✅ Testiraj Book - sve CRUD operacije + validacija
✅ Testiraj Author - sve CRUD operacije + validacija
✅ Testiraj Genre - sve CRUD operacije + validacija
✅ Testiraj Publisher - sve CRUD operacije + validacija
✅ Testiraj Review - sve CRUD operacije + validacija
```
