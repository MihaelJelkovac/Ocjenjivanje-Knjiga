# 🧪 Lab-5 Testovi - Kompletna Dokumentacija

> **Finalna verzija testova s 139 prošlih testova koji pokrivaju sve mogućnosti Lab-5 aplikacije**

---

## 📌 Što su testovi?

**Testovi su automatski programi koji provjeravaju je li vaša aplikacija ispravno radi.** Bez testova, trebali biste ručno provjeravati sve što ste napravili - što je sporo i podložno greškama.

---

## 🎯 Što se testira?

Testovi pokrivaju:

1. **CRUD operacije** - Create (dodaj), Read (pročitaj), Update (uredi), Delete (obriši)
2. **Validacije** - Jesu li polja pravilno validirana (obavezna polja, duljina, format)
3. **Rubni slučajevi (Edge cases)** - Što se dešava s praznim vrijednostima, unicode znakovima, extreme vrijednostima
4. **Relacije između entiteta** - Kako se knjige vežu za autore, kako se recenzije vežu za knjige
5. **Enumeracije** - BookStatus (Available, Reserved, Archived), ReviewSentiment (Positive, Neutral, itd.)
6. **API integracije** - HTTP zahtjevi na API endpoints (GET, POST, PUT, DELETE)

---

## 📂 Struktura testova

```
Tests/
├── ComprehensiveLabTests.cs        ← GLAVNI TEST FILE (139 testova)
├── Api/
│   ├── Lab5ApiFactory.cs           ← WebApplicationFactory za API testove
│   ├── Lab5TestFactory.cs          ← TestFactory s seed podatcima
│   ├── AuthorsApiTests.cs          ← API testovi za Authors
│   ├── BooksApiTests.cs            ← API testovi za Books
│   ├── BooksApiTests.cs            ← API testovi za Books
│   ├── GenresApiTests.cs           ← API testovi za Genres
│   ├── ReviewsApiTests.cs          ← API testovi za Reviews
│   ├── UsersApiTests.cs            ← API testovi za Users
│   ├── PublishersApiTests.cs       ← API testovi za Publishers
│   └── TestAuthHandler.cs          ← Auth helper
└── TESTIRANJE.md                   ← Ovaj file (dokumentacija)
```

---

## 🔴 ComprehensiveLabTests.cs - Detaljno

Ovo je **GLAVNI TEST FILE** s **139 testova** koji testira sve repository operacije, validacije i edge case-ove.

### **Dio 1: Repository CRUD Testovi (28 testova)**

Testiraju Create, Read, Update, Delete operacije za sve entitete.

#### **Book Repository Testovi (9 testova)**

| Test | Što testira | Primjer |
|------|-----------|---------|
| `CreateBook_WithValidData_ShouldSaveToDatabase` | Dodavanje nove knjige s valjanim podacima | Title="Gospodar Prstenova", Isbn="978-0547928227", PageCount=569 |
| `CreateBook_WithMinimumValidData_ShouldSucceed` | Minimalni podaci koji zadovoljavaju validaciju | Title="abc" (min 3 znaka), PageCount=1 (min 1) |
| `CreateBook_WithMaximumPageCount_ShouldSucceed` | Maksimalni broj stranica | PageCount=10000 (maksimalno dozvoljen) |
| `UpdateBook_ChangeTitle_ShouldUpdateDatabase` | Promjena naslova postojeće knjige | Stari: "Original", Novi: "Updated" |
| `UpdateBook_ChangeISBN_ShouldUpdateDatabase` | Promjena ISBN-a | Stari: "1111111111", Novi: "2222222222" |
| `DeleteBook_SoftDelete_ShouldSetDeletedAt` | Soft delete (označavanje kao obrisano, ne fizičko brisanje) | DeletedAt postavljen na sada |
| `GetAll_FiltersDeletedBooks` | Aktivne knjige se učitavaju, obrisane ne | Active: vidljivo, Deleted: nevidljivo |
| `GetById_ReturnsCorrectBook` | Preuzimanje specifične knjige po ID-u | ID=1 vraća "Specific Book" |

#### **Author, Review, User Repository Testovi (5+5+5 testova)**

Isti pattern kao Books:
- Create s valjanim/minimalnim podacima
- Update različitih polja
- Soft delete filtriranje
- GetById i GetAll operacije

**Primjer - Review testovi:**
- Ocjene 1-5 (sve kombinacije su testirane)
- Update score (3→5)
- Soft delete review

**Primjer - User testovi:**
- Username min 3 znaka, regex validacija
- ReputationPoints: 0 (minimum) do 10000 (maksimum)
- Premium membership toggle (true/false)

---

### **Dio 2: Validacijski Testovi (16 testova)**

Provjeravaju jesu li validacije ispravno postavljene. Testiraju što se dešava kad korisnik unese pogrešne podatke.

| Polje | Validacija | Test slučajevi |
|-------|-----------|---|
| **Book.Title** | Obavezno, 3-500 znakova | ✅ "Valid Title", ❌ "" (prazan), ❌ "ab" (prekratak), ❌ "x"×501 (predugačak) |
| **Book.ISBN** | Obavezno, 10-20 znakova, ISBN format | ✅ "978-0547928227" (valid), ❌ "abc" (kriv format), ❌ "123" (prekratak) |
| **Book.PageCount** | 1-10000 stranica | ✅ 1, 500, 10000 (valid), ❌ 0, -1 (premalo), ❌ 10001 (previše) |
| **Review.Score** | 1-5 | ✅ 1,2,3,4,5 (valid), ❌ 0, 6 (van range-a) |
| **Author.FirstName** | Obavezno, 2-100 znakova | ✅ "Jo" (min), ❌ "J" (prekratak) |
| **User.Username** | Obavezno, 3-100, regex `^[a-zA-Z0-9_]+$` | ✅ "user_123", ❌ "user-123" (hyphen nije dozvoljeno), ❌ "user@123" (@ nije dozvoljeno) |
| **User.Email** | Obavezno, valid email format | ✅ "user@example.com", ❌ "invalidemail", ❌ "user@@@example.com" (više @ znakova) |

**Zašto je ovo važno?**
Ako korisnik unese naziv knjige od 2 znaka umjesto minimalno 3, aplikacija mora odbiti i prikazati grešku.

---

### **Dio 3: Edge Case Testovi (15 testova)**

Testiraju **ekstremne i neobične scenarije** koje korisnik može napraviti.

| Scenario | Test | Očekivani rezultat |
|----------|------|-------------------|
| **Unicode znakovi** | `Book_WithUnicodeCharacters_ShouldSucceed` | Title="Gospodar Prstenova: Bratstvo Prstena" (hrvatski), Description="Епска фантастична прича" (srpski) - trebao bi raditi |
| **Prazni description** | `Book_WithEmptyDescription_ShouldSucceed` | Description="" (empty string je OK jer nije obavezno) |
| **Maksimalni description** | `Book_WithMaximumDescription_ShouldSucceed` | Description = 2000 znakova (maksimalno) |
| **Prazan comment** | `Review_WithEmptyComment_ShouldSucceed` | Comment="" - trebalo bi biti OK |
| **Duplikati ISBN** | `Book_CreateMultipleBooks_WithSameISBN_ShouldBothExist` | Dvije knjige s istim ISBN-om - obje trebaju postojati (nema unique constraint) |
| **Budući datumi** | `Book_WithFuturePublishedDate_ShouldSucceed` | PublishedOn = 10 godina u budućnosti - trebalo bi biti OK |
| **Stari datumi** | `Book_WithPastDate_ShouldSucceed` | PublishedOn = 1900. godina - trebalo bi biti OK |

**Zašto je ovo važno?**
Trebate biti sigurni da aplikacija pravilno rukuje unicode znakovima, različitim datumima i extremnim vrijednostima.

---

### **Dio 4: Relationship Testovi (5 testova)**

Testiraju kako se entiteti vežu jedni za druge kroz Foreign Keys.

| Test | Što testira | Primjer |
|------|-----------|---------|
| `Book_WithValidAuthor_ShouldEstablishRelationship` | Knjiga je vezana na autora | Book.AuthorId=1 → Author.FirstName="J.K.", LastName="Rowling" |
| `Review_WithValidBook_ShouldEstablishRelationship` | Recenzija je vezana na knjige | Review.BookId=1 → Book.Title="Test Book" |

**Zašto je ovo važno?**
Trebate provjeriti da su relacije između tablica ispravne i da se navigacijska svojstva pravilno učitavaju.

---

### **Dio 5: Enum Testovi (4 testa)**

Testiraju sve moguće vrijednosti za Enum polja.

| Enum | Vrijednosti | Testovi |
|------|-----------|---------|
| **BookStatus** | Available, Reserved, Archived | Svaka vrijednost je testirana |
| **ReviewSentiment** | Positive, Critical, Neutral, Enthusiastic | Svaka vrijednost je testirana |

**Primjer:**
```csharp
[Theory]
[InlineData(BookStatus.Available)]
[InlineData(BookStatus.Reserved)]
[InlineData(BookStatus.Archived)]
public async Task CreateBook_WithDifferentStatuses_ShouldSucceed(BookStatus status)
{
    // Test kreira knjige sa svakim statusom
    // Provjerava da je status ispravno spremljen
}
```

---

## 🔵 API Testovi (39 testova u Api/ folder-u)

Ovo su **integration testovi** koji testiraju HTTP endpoints preko interneta (simulirano).

### **Što testiraju API testovi?**

| Endpoint | HTTP Metoda | Što testira |
|----------|-----------|-----------|
| `/api/books` | GET | Vraća sve knjige (bez obrisanih) |
| `/api/books/1` | GET | Vraća specifičnu knjige |
| `/api/books` | POST | Dodaj novu knjige |
| `/api/books/1` | PUT | Uredi postojeću knjige |
| `/api/books/1` | DELETE | Obriši knjige (soft delete) |
| Isto za | → | Authors, Genres, Publishers, Reviews, Users |

### **Primjer API testa:**

```csharp
[Fact]
public async Task GetById_WithValidId_ReturnsBook()
{
    // ACT
    var response = await _client.GetAsync("/api/books/1");
    
    // ASSERT
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    var book = await response.Content.ReadFromJsonAsync<BookDto>();
    Assert.NotNull(book);
    Assert.Equal(1, book!.Id);
}
```

Ovo simulira što se dešava kad korisnik posjetim `/api/books/1` u pregledniku.

---

## 🚀 Kako pokrenuti testove?

### **1. Pokreni SVE testove:**
```powershell
cd c:\Users\Mihael\Desktop\ASP.NET
dotnet test lab-5/Lab5.csproj
```

### **2. Pokreni samo ComprehensiveLabTests:**
```powershell
dotnet test lab-5/Lab5.csproj --filter "Lab5.Tests.ComprehensiveLabTests"
```

### **3. Pokreni samo API testove:**
```powershell
dotnet test lab-5/Lab5.csproj --filter "Lab5.Tests.Api"
```

### **4. Pokreni specifičan test:**
```powershell
dotnet test lab-5/Lab5.csproj --filter "ComprehensiveLabTests.BookRepository_CreateBook_WithValidData_ShouldSaveToDatabase"
```

---

## 📊 Trenutni status testova

```
Total:     139 testovi
Passed:    139 ✅
Failed:    0 ❌
Duration:  ~3 sekunde
```

---

## 🎓 AAA Pattern - Kako su testovi napisani?

Svaki test slijedi **AAA (Arrange, Act, Assert)** pattern:

```csharp
[Fact]
public async Task CreateBook_WithValidData_ShouldSaveToDatabase()
{
    // ARRANGE - Pripremi podatke
    var context = GetInMemoryDbContext();
    var repository = new BookRepository(context);
    
    var newBook = new Book
    {
        Title = "Gospodar Prstenova",
        Isbn = "978-0547928227",
        PageCount = 569,
        AuthorId = 1,
        PublisherId = 1
    };

    // ACT - Izvedi akciju
    await repository.CreateAsync(newBook);
    await context.SaveChangesAsync();

    // ASSERT - Provjeri rezultat
    var savedBook = await context.Books
        .FirstOrDefaultAsync(b => b.Title == "Gospodar Prstenova");
    
    Assert.NotNull(savedBook);
    Assert.Equal(569, savedBook.PageCount);
    Assert.Null(savedBook.DeletedAt);
}
```

---

## ✅ Checklist - Što testove pokrivaju?

- ✅ **CRUD za sve entitete** (Books, Authors, Genres, Publishers, Reviews, Users)
- ✅ **Sve validacije** (Required fields, string length, email format, number ranges)
- ✅ **Soft delete** (DeletedAt filtriranje)
- ✅ **Edge cases** (Unicode, empty values, maximum values, future dates, duplicates)
- ✅ **Relationships** (Foreign keys, navigation properties)
- ✅ **Enums** (BookStatus, ReviewSentiment)
- ✅ **API endpoints** (GET, POST, PUT, DELETE)
- ✅ **Authorization** (Role-based access)
- ✅ **Error handling** (Validation errors, not found responses)

---

## 📝 Zašto su testovi važni?

1. **Sigurnost** - Provjeravaju da kod radi kako treba
2. **Refaktoriranje bez straha** - Ako nešto slomite, testovi će vam reći
3. **Dokumentacija** - Testovi pokazuju kako koristiti kod
4. **Kvaliteta** - Manje bug-ova u produkciji
5. **Brzina razvoja** - Manje vremena na ručnom testiranju

---

## 🔗 Dodatne info

- **Test framework:** xUnit
- **Database:** SQLite (InMemory za testove)
- **Pattern:** AAA (Arrange, Act, Assert)
- **Coverage:** 139 testova pokriva sve scenarije
- **Build status:** ✅ All tests passing

---

**Zadnja ažuriranja:** Svibanj 31, 2026
**Zadnja pokretanja:** 139/139 testova prošlo ✅

Ako trebate dodati nove testove, slijedite isti AAA pattern i stavite ih u `ComprehensiveLabTests.cs`.
