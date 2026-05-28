# Lab 4 - Testovi

Ovaj folder sadrži sve testove za Lab 4 projekat.

## 📁 Struktura

```
Tests/
├── BookRepositoryTests.cs       - Testovi za Book CRUD operacije
├── AuthorRepositoryTests.cs     - (kreiraj po istom obrascu)
├── GenreRepositoryTests.cs      - (kreiraj po istom obrascu)
├── PublisherRepositoryTests.cs  - (kreiraj po istom obrascu)
├── ReviewRepositoryTests.cs     - (kreiraj po istom obrascu)
└── README.md                    - Ovaj file
```

## 🚀 Kako Početi

### 1. Instaliraj Test Pakete

```powershell
cd c:\Users\Mihael\Desktop\ASP.NET\lab-4

# Instalira xUnit, InMemory bazu i ostalo
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
dotnet add package Microsoft.EntityFrameworkCore.InMemory
```

### 2. Pokreni BookRepositoryTests

```powershell
dotnet test Tests/BookRepositoryTests.cs
# ili sve testove odjednom
dotnet test
```

### 3. Kreiraj Testove za Ostale Entitete

Kopiraj `BookRepositoryTests.cs` i zamijeni:
- `Book` → `Author`, `Genre`, `Publisher`, `Review`
- `BookRepository` → `AuthorRepository`, itd.
- Prilagodi svojstva modela

## 📋 Testni Scenariji (AAA Pattern)

Svaki test slijedi AAA pattern:

```csharp
[Fact]
public async Task TestName_Scenario_ExpectedResult()
{
    // ARRANGE - Pripremi podatke
    
    // ACT - Izvrši akciju
    
    // ASSERT - Provjeri rezultat
}
```

## ✅ Checklist - Što Trebam Testirati

- [ ] CRUD Create - spremi novu entitetu
- [ ] CRUD Read - učita entitetu po ID-u
- [ ] CRUD Update - ažurira postojeću entitetu
- [ ] CRUD Delete - soft delete postavlja DeletedAt
- [ ] Validacija - obavezna polja
- [ ] Filter - DeletedAt = null ne prikazuje obrisane
- [ ] Autocomplete - pretraga vraća filtrirane rezultate

## 📚 Ressursi

- Skill: `../.github/skills/Lab5-testing-skill/SKILL.md`
- Xunit dokumentacija: https://xunit.net/
- EF Core InMemory: https://docs.microsoft.com/en-us/ef/core/providers/in-memory

## 💡 Savjet

Testiraj prvo jednu entitetu (Book) potpuno. Kada su ti testovi savršeni, lako možeš replicirati pattern na ostale entitete.

