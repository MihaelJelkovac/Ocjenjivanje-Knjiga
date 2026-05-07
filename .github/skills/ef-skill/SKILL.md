---
description: "Entity Framework (EF) Core operations - add/modify entities, run migrations, seed data"
applyTo:
  - "**/*DbContext.cs"
  - "**/Models/*.cs"
  - "**/Migrations/**"
---

# Entity Framework (EF) Core Skill - Lab 3

Ovaj skill pružava smjernice za rad s Entity Framework Core u Lab 3 projektu.

## Kada koristiti ovaj skill

- Trebate dodati novu tablicu/entitet u bazu podataka
- Trebate promijeniti postojeću tablicu (npr. dodati novo polje)
- Trebate pokrenuti migraciju
- Trebate ažurirati bazu podataka
- Trebate dodati novu relaciju između tablica

## Važne naredbe

### 1. Pokretanje migracija (uvijek iz lab-3 direktorija)

```powershell
cd c:\Users\Mihael\Desktop\ASP.NET\lab-3

# Kreiraj novu migraciju s opisnim nazivom
dotnet ef migrations add AddNewFeature

# Primjeni migraciju na bazu
dotnet ef database update

# Prikaži listu svih migracija
dotnet ef migrations list
```

### 2. Uključivanje podataka pri konfiguraciji (DbContext.OnModelCreating)

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Seeding podataka
    modelBuilder.Entity<Author>().HasData(
        new Author { Id = 1, FirstName = "Stephen", LastName = "King", ... }
    );
}
```

### 3. Konfiguracija relationship-a

```csharp
// 1-N veza (jedan autor, mnogu knjiga)
modelBuilder.Entity<Book>()
    .HasOne(b => b.Author)
    .WithMany(a => a.Books)
    .HasForeignKey(b => b.AuthorId);

// N-N veza (mnogo knjiga, mnogo žanrova)
modelBuilder.Entity<BookGenre>()
    .HasKey(bg => new { bg.BookId, bg.GenreId });
```

---

## Korak-po-korak: Dodavanje novog entiteta

### Korak 1: Kreiraj modelsku klasu

```csharp
// Models/Award.cs
using System.ComponentModel.DataAnnotations;

namespace Lab3.Models;

public class Award
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime AwardedOn { get; set; }

    // Foreign key
    [ForeignKey(nameof(Book))]
    public int BookId { get; set; }

    public virtual Book Book { get; set; } = default!;
}
```

### Korak 2: Dodaj DbSet u DbContext

```csharp
// Data/CatalogDbContext.cs
public DbSet<Award> Awards { get; set; }
```

### Korak 3: Ažuriraj model relationships (ako je potrebno)

```csharp
// Models/Book.cs
public virtual ICollection<Award> Awards { get; set; } = new List<Award>();
```

### Korak 4: Konfiguriraj relationships u DbContext.OnModelCreating

```csharp
modelBuilder.Entity<Award>()
    .HasOne(a => a.Book)
    .WithMany(b => b.Awards)
    .HasForeignKey(a => a.BookId)
    .OnDelete(DeleteBehavior.Cascade);
```

### Korak 5: Kreiraj migraciju

```powershell
dotnet ef migrations add AddAwardEntity
```

### Korak 6: Primjeni migraciju

```powershell
dotnet ef database update
```

---

## Korak-po-korak: Dodavanje novog polja na postojeći entitet

### Korak 1: Dodaj svojstvo u modelsku klasu

```csharp
// Models/Book.cs
public class Book
{
    // ...
    public string Genre { get; set; } = string.Empty;  // Novo polje
}
```

### Korak 2: Kreiraj migraciju (EF će automatski detektirati promjenu)

```powershell
dotnet ef migrations add AddGenreFieldToBook
```

### Korak 3: Pregledaj migracijsku datoteku

```powershell
# Migracija će biti u Migrations/ foldera
# Trebao bi viditi AddColumn komandu za novo polje
```

### Korak 4: Primjeni migraciju

```powershell
dotnet ef database update
```

---

## Često postavljana pitanja (FAQ)

### P: Kako resetiram bazu?

```powershell
# Obriši bazu datoteke
Remove-Item catalog.db

# Primjeni sve migracije ispočetka
dotnet ef database update
```

### P: Kako vidim SQL skripte koje EF generira?

```powershell
# Izvezi migracije kao SQL script
dotnet ef migrations script
```

### P: Što ako migracija ne uspije primjenjivanja na bazu?

```powershell
# Ukloni zadnju migraciju (ako je neuspješna)
dotnet ef migrations remove

# Ili resetiraj bazu i kreni ispočetka
Remove-Item catalog.db
dotnet ef database update
```

---

## Best Practices

✓ **Koristite async-await** pri pozivu baze u kontrolerima
✓ **Koristi Include()** pri učitavanju povezanih entiteta
```csharp
var book = await _context.Books
    .Include(b => b.Author)
    .Include(b => b.Reviews)
    .FirstOrDefaultAsync(b => b.Id == id);
```

✓ **Izbjegavaj N+1 problema** s Include i ThenInclude
✓ **Koristi virtual svojstva** za lazy loading relacija
✓ **Postavi DeleteBehavior** na Cascade za važne veze
```csharp
.OnDelete(DeleteBehavior.Cascade);
```

✓ **Provjerit migracije prije commitiranja**
```powershell
dotnet ef migrations list
```

---

## Datoteke važne za EF

| Datoteka | Uloga |
|----------|-------|
| `Data/CatalogDbContext.cs` | DbContext - veza s bazom |
| `Models/*.cs` | Modelske klase - reprezentacija tablica |
| `Migrations/` | Migracijske skripte |
| `appsettings.json` | Connection string |

---

## SQL Lite specifičnosti

- Podržava foreignke keys s `PRAGMA foreign_keys = ON`
- Ne podržava sve SQL tipove kao MSSQL
- Optimalna za development i male projekte

---

## Primjer kompletan workflow

```powershell
cd c:\Users\Mihael\Desktop\ASP.NET\lab-3

# 1. Promijeni model (npr. dodaj novo polje)
# 2. Kreiraj migraciju
dotnet ef migrations add DescriptiveChangeName

# 3. Pregledaj datoteku Migrations/XXX_DescriptiveChangeName.cs
# (Opciono) Uređuj ako EF nije detektirao sve ispravno

# 4. Primjeni promjene na bazu
dotnet ef database update

# 5. Testiraj aplikaciju
dotnet run
```

---

**Verzija**: 1.0  
**Zadnja ažurenja**: 2026-05-07  
**Autor**: GitHub Copilot
