# Lab-5 Testovi - README

## 📌 Što je ovo?

Ovdje se nalaze **svi testovi za Lab-5 projekt**. Testovi provjeravaju:
- ✅ CRUD operacije (Create, Read, Update, Delete)
- ✅ Validacije (obavezna polja, duljina, format)
- ✅ Edge case-ovi (unicode, extreme vrijednosti)
- ✅ Relacije između entiteta
- ✅ API endpoints

---

## 🎯 Testovi na brzinu

```
Total:     139 testova
Passed:    139 ✅ 
Failed:    0 ❌
Duration:  ~3 sekunde
```

---

## 📂 Struktura

```
Tests/
├── ComprehensiveLabTests.cs     ← GLAVNI TEST FILE (Unit testovi, 139)
├── Api/
│   ├── Lab5ApiFactory.cs        ← Factory za API testove
│   ├── Lab5TestFactory.cs       ← Test factory s seed podacima
│   ├── AuthorsApiTests.cs       ← API testovi
│   ├── BooksApiTests.cs         ← API testovi
│   ├── GenresApiTests.cs        ← API testovi
│   ├── ReviewsApiTests.cs       ← API testovi
│   ├── UsersApiTests.cs         ← API testovi
│   ├── PublishersApiTests.cs    ← API testovi
│   └── TestAuthHandler.cs       ← Auth helper
└── TESTIRANJE.md                ← Detaljno objašnjenje testova ⭐
```

---

## 🚀 Kako pokrenuti testove?

### Pokreni sve testove:
```powershell
cd c:\Users\Mihael\Desktop\ASP.NET
dotnet test lab-5/Lab5.csproj
```

### Pokreni samo ComprehensiveLabTests:
```powershell
dotnet test lab-5/Lab5.csproj --filter "ComprehensiveLabTests"
```

### Pokreni samo specifičan test:
```powershell
dotnet test lab-5/Lab5.csproj --filter "CreateBook_WithValidData"
```

---

## 📚 Što testovi pokrivaju?

### Dio 1: Repository Testovi (28)
- **Books** (9) - Create, Update, Delete, GetById, GetAll
- **Authors** (5)
- **Reviews** (5)
- **Users** (5)
- **Genres & Publishers** (4)

### Dio 2: Validacijski Testovi (16)
- Title validacija (required, length)
- ISBN validacija (format)
- PageCount validacija (range)
- Email validacija
- Username validacija (regex)
- Score validacija (1-5)

### Dio 3: Edge Case Testovi (15)
- Unicode znakovi (Hrvatski, Srpski)
- Prazne vrijednosti
- Maksimalne vrijednosti
- Duplikati
- Prošli i budući datumi

### Dio 4: Relationship Testovi (5)
- Book→Author FK
- Review→Book FK
- Navigation properties

### Dio 5: Enum Testovi (4)
- BookStatus (Available, Reserved, Archived)
- ReviewSentiment (Positive, Critical, Neutral, Enthusiastic)

### Dio 6: API Integration Testovi (39)
- GET /api/books, /api/authors, itd.
- POST, PUT, DELETE operacije
- Authorization validation
- Error handling

---

## 💡 AAA Pattern - Kako su testovi napisani?

```csharp
[Fact]
public async Task CreateBook_WithValidData_ShouldSaveToDatabase()
{
    // ARRANGE - Pripremi
    var context = GetInMemoryDbContext();
    var newBook = new Book { Title = "...", ... };

    // ACT - Izvedi
    await repository.CreateAsync(newBook);

    // ASSERT - Provjeri
    Assert.NotNull(savedBook);
}
```

---

## 📖 Za više detalja

👉 **Vidi [TESTIRANJE.md](TESTIRANJE.md)** za detaljno objašnjenje što svaki test radi i zašto je važan.

---

## ✨ Korisne info

- **Test framework:** xUnit
- **Database:** SQLite (InMemory za testove)
- **Build:** ✅ Sve kompajlira bez greške
- **Status:** ✅ Svi testovi prolaze

---

Ako trebate dodati nove testove, dodajte ih u `ComprehensiveLabTests.cs` i slijedite isti AAA pattern.

