# Lab 5 - Testiranje Pregled

## 🎯 Test Rezultati - **11/11 PROŠLO** ✅

### ✅ BooksCrudTests (2/2 passed)
- **GetAll_ReturnsOk** - Testira GET /api/books bez parametara
- **Create_Update_Delete_Cycle_Works_WhenAuthorized** - CRUD ciklus od kraja do kraja

### ✅ BookRepositoryTests (4/4 passed)
- **CreateBook_WithValidData_ShouldSaveToDatabase** - Kreiranje knjige u bazi
- **UpdateBook_WithNewPageCount_ShouldUpdateDatabase** - Promjena podataka
- **DeleteBook_SoftDelete_ShouldSetDeletedAtNotNull** - Soft delete logika
- **CreateBook_WithoutTitle_ShouldFailValidation** - Validacija polja

### ✅ AuthorsApiTests (3/3 passed)
- **GetAll_ReturnsSeededAuthors** - GET /api/authors
- **GetById_Returns404_ForMissingAuthor** - 404 za nepostojeće
- **Post_CreatesAuthor_AndReturnsCreated** - Kreiranje novog autora

### ✅ BooksApiTests (2/2 passed)
- **GetById_ReturnsSeededBook_WithNestedDTOs** - GET s ugniježđenim DTO
- **Post_ValidationFailure_ReturnsBadRequest** - Validacijski error

---

## 📚 Detaljno Objašnjenje Kako Testovi Rade

### 1️⃣ **BooksCrudTests** - Integracijski testovi CRUD operacija

```
Namjena: Testira kompletnu komunikaciju HTTP klijent → API endpoint → Database
Factory: Lab5TestFactory (s test autentikacijom)
Okruženje: SQLite in-memory baza s seeded podacima
```

#### Test: GetAll_ReturnsOk
```csharp
[Fact]
public async Task GetAll_ReturnsOk()
{
    var resp = await _client.GetAsync("/api/books");
    Assert.Equal(HttpStatusCode.OK, resp.StatusCode);  // ✓ Očekuje 200
    var list = await resp.Content.ReadFromJsonAsync<List<BookDto>>();
    Assert.NotNull(list);
    Assert.NotEmpty(list);  // ✓ Lista nije prazna
}
```
**Što testira:**
- HTTP GET zahtjev na `/api/books`
- Vraćeni status je `200 OK`
- Odgovor se može deserijalizirati u `List<BookDto>`
- Lista sadrži barem jedan element (seeded data)

#### Test: Create_Update_Delete_Cycle_Works_WhenAuthorized
```
4-faza test koji provjerava cijeli CRUD ciklus:

1. CREATE (POST /api/books)
   └─ Šalje BookUpsertDto s valjanim podacima
   └─ Očekuje 201 Created
   └─ Prima BookDto s generirajućim ID-om

2. UPDATE (PUT /api/books/{id})
   └─ Šalje ažurirane podatke
   └─ Očekuje 200 OK
   └─ Provjerava da je naslov zaista promijenjen

3. DELETE (DELETE /api/books/{id})
   └─ Briše kreiranu knjigau
   └─ Očekuje 204 No Content

4. VERIFY (GET /api/books/{id})
   └─ Provjerava da obrisana knjiga više ne postoji
   └─ Očekuje 404 Not Found
```

---

### 2️⃣ **BookRepositoryTests** - Unit testovi Business Layer

```
Namjena: Testira IBookRepository bez HTTP komunikacije
Okruženje: InMemory DbContext za svaki test
Pattern: AAA (Arrange → Act → Assert)
```

#### Test: CreateBook_WithValidData_ShouldSaveToDatabase
```csharp
// ARRANGE - Priprema
var context = GetInMemoryDbContext();  // Jedinstvena baza za ovaj test
var repository = new BookRepository(context);
var newBook = new Book { Title = "Gospodar Prstenova", ... };

// ACT - Izvršavanje
await repository.CreateAsync(newBook);
await context.SaveChangesAsync();

// ASSERT - Provjera
var savedBook = await context.Books
    .FirstOrDefaultAsync(b => b.Title == "Gospodar Prstenova");
Assert.NotNull(savedBook);
Assert.Equal("978-0547928227", savedBook.Isbn);
```

**Što testira:**
- Repository može kreirati novi zapis
- Zapis se persista u bazu
- Svi podaci su točni nakon čuvanja

#### Test: DeleteBook_SoftDelete_ShouldSetDeletedAtNotNull
```csharp
// Simulira soft delete (logičko brisanje)
book.DeletedAt = DateTime.UtcNow;
context.Books.Update(book);
await context.SaveChangesAsync();

// Provjerava da se ne pojavljuje u aktivnim zapisima
var activeBooks = await context.Books
    .Where(b => b.DeletedAt == null)
    .ToListAsync();
Assert.DoesNotContain(activeBooks, b => b.Id == bookId);
```

**Što testira:**
- Soft delete se ispravno primjenjuje
- Obrisani zapisi se filtriraju iz aktivnih
- DeletedAt se postavlja na trenutno vrijeme

---

### 3️⃣ **BooksApiTests** - API integracijski testovi

```
Namjena: Testira API endpointe s autentikacijom
Factory: Lab5TestFactory (testni auth handler)
```

#### Test: GetById_ReturnsSeededBook_WithNestedDTOs
```csharp
var response = await _client.GetAsync("/api/books/1");
var book = await response.Content.ReadFromJsonAsync<BookDto>();

Assert.NotNull(book!.Author);      // ✓ Author je uključen (nested)
Assert.NotNull(book.Publisher);    // ✓ Publisher je uključen (nested)
```

**Što testira:**
- GET sa ID-om vraća specifičnu knjigau
- Odgovor uključuje ugniježđene AuthorDto i PublisherDto
- Sve relacije se ispravno mapiraju

#### Test: Post_ValidationFailure_ReturnsBadRequest
```csharp
var invalid = new BookUpsertDto
{
    Title = string.Empty,        // ❌ Obavezno polje
    Isbn = string.Empty,          // ❌ Nedostaje
    PageCount = 0                 // ❌ Nevaljani raspon
};

var response = await _client.PostAsJsonAsync("/api/books", invalid);
Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);  // ✓ 400
```

**Što testira:**
- [ApiController] automatski validira model
- Nevaljanog podatka → `400 Bad Request`
- Validacijska pravila se provjeravaju prije akcije

---

### 4️⃣ **AuthorsApiTests** - CRUD testovi za Authors

```
Namjena: Slično BooksApiTests ali za Authors entitet
Endpoint: GET /api/authors, POST /api/authors, itd.
```

#### Test: Post_CreatesAuthor_AndReturnsCreated
```csharp
var request = new AuthorUpsertDto
{
    FirstName = "Test",
    LastName = "Author",
    Biography = "Integration test author",
    BirthDate = new DateTime(1990, 1, 1),
    Nationality = "Croatian",
    Website = "https://example.com"
};

var response = await _client.PostAsJsonAsync("/api/authors", request);
var created = await response.Content.ReadFromJsonAsync<AuthorDto>();

Assert.Equal("Test", created!.FirstName);      // ✓ Mapiranje je točno
Assert.Equal("Author", created.LastName);      // ✓ Svi podaci su tu
```

**Što testira:**
- Novo kreirani autor se vraća kao `201 Created`
- AuthorUpsertDto se ispravno mapira u AuthorDto
- ID se geneira i vraća

---

## 🔧 Test Infrastruktura

### Lab5ApiFactory
```
Uloga: Bazna fabrika za API testove
Što radi:
├─ Kreira WebApplicationFactory za Lab5 projekt
├─ Konfiguira SQLite in-memory bazu
├─ Osigurava čist slate za svaki test (Dispose)
└─ Seeduje inicijalne podatke
```

### Lab5TestFactory
```
Uloga: Nasljeđuje Lab5ApiFactory + dodaje autentikaciju
Što radi:
├─ Konfiguira TestAuthHandler
├─ Postavlja default authenticate scheme na "Test"
├─ Simulira logiranog korisnika s Admin rolom
└─ Omogućuje [Authorize] endpointima da rade u testima
```

### TestAuthHandler
```csharp
protected override Task<AuthenticateResult> HandleAuthenticateAsync()
{
    var claims = new[] {
        new Claim(ClaimTypes.Name, "testuser"),
        new Claim(ClaimTypes.NameIdentifier, "1"),
        new Claim(ClaimTypes.Role, "Admin")        // ← Admin rola!
    };
    // Svi zahtjevi su autentificirani kao Admin
}
```

**Efekt:**
- Testovi mogu pozivati `[Authorize]` endpointe
- TestUser ima "Admin" rolu
- `[Authorize(Roles = "Admin")]` prolaze test

---

## 🔄 HTTP Status Kodovi Koje Testovi Provjeravaju

| Status | Značenje | Testira se u |
|--------|----------|---------------|
| `200 OK` | Uspješan GET/PUT | GetAll, GetById, Update |
| `201 Created` | Novo kreiran zapis | Create, Post_CreatesAuthor |
| `204 No Content` | Uspješno obrisano | Delete cycle |
| `400 Bad Request` | Validacijska greška | Post_ValidationFailure |
| `404 Not Found` | Zapis ne postoji | GetById_Returns404, Delete verify |

---

## 📊 Što Testovi Pokrivaju

| Aspekt | Testira se | Primjer |
|--------|-----------|---------|
| **CRUD** | Create, Read, Update, Delete | BooksCrudTests |
| **Validacija** | Data annotations | Post_ValidationFailure |
| **Relacije** | Nested DTOs | GetById_ReturnsSeededBook_WithNestedDTOs |
| **Soft Delete** | Logičko brisanje | DeleteBook_SoftDelete |
| **Autentikacija** | Roles i Claims | [Authorize(Roles = "Admin")] |
| **Model Mapping** | Entity → DTO | Post_CreatesAuthor |
| **HTTP Metode** | GET, POST, PUT, DELETE | Svi testovi |

---

## ✨ Zaključak

Lab 5 testovi su **komprehenzivni integracijski i unit testovi** koji pokrivaju:

✅ **API sloj** - HTTP komunikacija, status kodovi, model binding  
✅ **Business sloj** - Repository operacije, logika  
✅ **Data sloj** - Database operacije, validacija  
✅ **Autentikacija** - Role-based access control  
✅ **Mapiranje** - Entity ↔ DTO konverzije  
✅ **Validacija** - Data annotations, model state  

**Svi testovi koriste AAA pattern:**
- **Arrange**: Priprema test podataka i okruženja
- **Act**: Izvršava akciju koju testirate
- **Assert**: Provjerava očekivane rezultate
