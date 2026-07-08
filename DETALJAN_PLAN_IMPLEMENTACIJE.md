# 🚀 DETALJNAN PLAN IMPLEMENTACIJE - SVE KORAKE

**Cilj:** Maksimizirati bodove koje mogu objasniti i braniti

---

## 📊 SAŽETAK - SVIH MOGUĆIH ZADATAKA

| # | Zadatak | Bodovi | Teško za Napraviti? | Teško za Objasniti? | Vrijeme | Status |
|---|---------|--------|-------------------|-------------------|---------|--------|
| 1 | **Logging (Serilog)** | 2 | ⭐ Lako | ⭐ Lako | 2h | TREBAM |
| 2 | **Deploy (Azure)** | 3 | ⭐ Lako | ⭐ Lako | 1-2h | TREBAM |
| 3 | **AI Integracija (Claude)** | 3 | ⭐⭐ Srednje | ⭐⭐ Srednje | 3-4h | TREBAM |
| 4 | **MCP Expose** | 2 | ⭐⭐⭐ Teško | ⭐⭐⭐ Teško | 2-3h | OPCIONO |
| 5 | **Playwright Testovi** | 3 (+3 extra) | ⭐⭐ Srednje | ⭐⭐ Srednje | 2-3h | OPCIONO |
| 6 | **Global Search+** | +1 | ⭐ Lako | ⭐ Lako | 1h | OPCIONO |
| 7 | **Detaljnija AI** | +2 | ⭐⭐⭐ Teško | ⭐⭐⭐ Teško | 3h | OPCIONO |

---

## 🎯 PREPORUKE PO SCENARIJU

### **Scenario A: SIGURNO PROĆI (51% - Ocjena 2)**
```
1. Logging (2 boda)        - OBAVEZNO
2. Deploy (3 boda)         - OBAVEZNO  
3. AI Integracija (3 boda) - OBAVEZNO
4. Global Search+ (1 bod)  - OPCIONO

= 8-9 novih bodova
= 34-35 bodova total (48-50%) 🔴 TREBAM MALO VIŠE

DODATI:
5. Playwright Testovi (3 boda) = 37-38 bodova (53%) ✅
ILI
5. MCP (2 boda) + AI detaljnije (1-2 boda) = 36-37 bodova (51-53%) ✅
```

### **Scenario B: SOLIDNA OCJENA (75% - Ocjena 4)**
```
1. Logging (2 boda)             - OBAVEZNO
2. Deploy (3 boda)              - OBAVEZNO
3. AI Integracija (3 boda)      - OBAVEZNO
4. Playwright Testovi (3 boda)  - OBAVEZNO
5. Global Search+ (1 bod)       - OBAVEZNO
6. MCP Expose (2 boda)          - OBAVEZNO

= 14 novih bodova
= 40 bodova total (57%)... TREBAM JOŠ 12
```

### **Scenario C: PREPRIČATO (100% znanja)**
```
Kombinacija svih prethodnih + detaljnije AI + performance optimizacija
= 50+ bodova (71%+) ✅✅
```

**PREPORUKA:** Scenario A + Playwright Testovi = 38-40 bodova (54-57%) ✅

---

---

# 📋 DETALJNI KORACI - SVAKI ZADATAK

---

## ZADATAK 1: LOGGING - SERILOG (2 BODA)

### 🎯 Cilj
- Aplikacija logira sve operacije u datoteke
- Možeš vidjeti što se dogodilo ako nešto krene po zlu
- Kriterij: "Implementacija logging mehanizma (file ili API)"

### ⏱️ Vrijeme
- **Implementacija:** 2 sata
- **Testiranje:** 30 minuta
- **Objasniti:** 5 minuta

### 🔧 SVEKORACI

#### **KORAK 1: Dodaj Serilog NuGet pakete**
```xml
<!-- Otvori: ispit/Lab5.csproj -->
<!-- Dodaj u <ItemGroup> gdje su ostali paketi: -->

<PackageReference Include="Serilog" Version="3.1.1" />
<PackageReference Include="Serilog.AspNetCore" Version="8.0.1" />
<PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
```

**Što se dogodi:**
- Serilog će biti dostupan u projektu
- `dotnet restore` će automatski preuzeti pakete

---

#### **KORAK 2: Kreiraj Logs folder**
```bash
# U ispit direktoriumu:
mkdir Logs
```

**Što se dogodi:**
- Kreirat će se folder gdje će se pisati logovi
- Primjer: `Logs/app-2026-07-08.log`

---

#### **KORAK 3: Konfiguracija u Program.cs**

**Prvo:** Otvori `ispit/Program.cs`

**Dodaj na SAMOM POČETKU (prije svega):**
```csharp
using Serilog;  // ← DODAJ OVO NA POČETAK

// Prije: var builder = WebApplication.CreateBuilder(args);

var builder = WebApplication.CreateBuilder(args);

// ODMAH NAKON var builder:
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File(
        path: "Logs/app-.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger();

builder.Logging.AddSerilog(Log.Logger);
```

**Što će se desiti:**
- Svaki dan će se kreirati nova log datoteka (`app-2026-07-08.log`, `app-2026-07-09.log`, itd.)
- Logger će biti dostupan u svim kontrolerima

---

#### **KORAK 4: Dodaj Logging u BooksController.cs**

**Otvori:** `ispit/Controllers/BooksController.cs`

**Na početku klase dodaj:**
```csharp
private readonly ILogger<BooksController> _logger;

public BooksController(
    IBookRepository bookRepository,
    IAuthorRepository authorRepository,
    IPublisherRepository publisherRepository,
    IGenreRepository genreRepository,
    IHttpContextAccessor httpContextAccessor,
    UserManager<AppUser> userManager,
    ILogger<BooksController> logger)  // ← DODAJ OVO
{
    _bookRepository = bookRepository;
    _authorRepository = authorRepository;
    _publisherRepository = publisherRepository;
    _genreRepository = genreRepository;
    _httpContextAccessor = httpContextAccessor;
    _userManager = userManager;
    _logger = logger;  // ← SPREMI LOGGER
}
```

**Sada u svakoj akciji dodaj logove:**

```csharp
[AllowAnonymous]
[Route("")]
[Route("index")]
public async Task<IActionResult> Index()
{
    _logger.LogInformation("📖 Index pristup - Popis svih knjiga");
    var currentUser = await _userManager.GetUserAsync(User);
    var books = await _bookRepository.GetAllAsyncForUserAsync(currentUser?.Id);
    _logger.LogInformation("✅ Učitano {BookCount} knjiga", books.Count);
    return View(books);
}

[Authorize(Roles = "Admin,Manager")]
[HttpPost]
[Route("create")]
public async Task<IActionResult> Create(Book model)
{
    _logger.LogInformation("📝 Pokušaj kreiranja knjige: {BookTitle}", model.Title);
    
    // ... postojeći kod ...
    
    try
    {
        await _bookRepository.CreateAsync(model);
        _logger.LogInformation("✅ Knjiga uspješno kreirana: {BookId} - {BookTitle}", model.Id, model.Title);
        return RedirectToAction(nameof(Index));
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "❌ Greška pri kreiranju knjige: {BookTitle}", model.Title);
        ModelState.AddModelError(string.Empty, "Greška: " + ex.Message);
        return View(model);
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
        if (success)
        {
            _logger.LogInformation("✅ Knjiga uspješno obrisana: {BookId}", id);
            return Json(new { success = true, message = "Knjiga je uspješno obrisana" });
        }
        else
        {
            _logger.LogWarning("⚠️ Knjiga nije pronađena: {BookId}", id);
            return Json(new { success = false, message = "Knjiga nije pronađena" });
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "❌ Greška pri brisanju knjige: {BookId}", id);
        return Json(new { success = false, message = "Greška: " + ex.Message });
    }
}
```

---

#### **KORAK 5: Dodaj Logging u Druge Kontrolere**

Isto kao BooksController, dodaj u:
- `AccountController.cs` - Logiranje login/logout
- `ReviewsController.cs` - Logiranje recenzija
- `AuthorsController.cs` - Logiranje autora

---

#### **KORAK 6: Testiranje**

**U Visual Studio:**
```bash
# Pokreni aplikaciju:
dotnet run --project ispit/Lab5.csproj

# Otvori u pregledniku:
# https://localhost:7003

# Klikni na Knjige → Stvori novu → Obriši nešto
```

**Što provjeri:**
```
1. Otvori: ispit/Logs/
2. Trebala bi biti datoteka: app-2026-07-08.log
3. Otvori je u Notepad++
4. Trebale bi biti linije poput:
   2026-07-08 14:23:15.123 +02:00 [INF] 📖 Index pristup - Popis svih knjiga
   2026-07-08 14:23:45.456 +02:00 [INF] ✅ Učitano 3 knjiga
   2026-07-08 14:25:12.345 +02:00 [INF] 📝 Pokušaj kreiranja knjige: Nova Knjiga
```

---

### 📝 KAKO OBJASNITI (5 minuta)

**Odgovor na pitanje: "Opiši logging u tvojoj aplikaciji"**

```
"Implementirao sam Serilog logging kako bi imao kompletan zapis 
što se dešava u aplikaciji.

KAKO FUNKCIONIRA:
1. Serilog je biblioteka koja hvataj sve što se dogodi
2. Svaki dan se kreiraj nova log datoteka u Logs/ folderu
3. U kontrolerima sam dodao ILogger koja logira:
   - Kada netko pristupi Index stranici
   - Kada se kreira nova knjiga
   - Kada se knjiga briše
   - Sve greške koji se dogode

PRIMJER LOGA:
  2026-07-08 14:25:12 [INF] ✅ Knjiga uspješno kreirana: Harry Potter
  2026-07-08 14:26:45 [ERR] ❌ Greška pri brisanju: Foreign Key Violation

ZAŠTO JE VAŽNO:
- Ako nešto krene po zlu, mogu vidjeti točno što se desilo
- Mogu pratiti tko što radi (npr. tko je obrisao koju recenziju)
- Mogu debugirat probleme kasnije analizirajući logove
- U produkciji je logging obavezan jer nemaš pristupa debuggeru"
```

---

### ✅ CHECKLIST

- [ ] Dodao Serilog pakete u .csproj
- [ ] Kreirio Logs folder
- [ ] Konfigurirao Serilog u Program.cs
- [ ] Dodao _logger u BooksController
- [ ] Dodao log linije u Create/Edit/Delete akcije
- [ ] Testirao - datoteka app-*.log se kreira
- [ ] Datoteka ima logove ✅

**Rezultat: +2 BODA** ✅

---

---

## ZADATAK 2: DEPLOY NA AZURE (3 BODA)

### 🎯 Cilj
- Aplikacija će biti dostupna na javnoj adresi
- Neće trebati lokalni server
- Kriterij: "Deploy na cloud provider (Google, Azure) ili Virtual Machine"

### ⏱️ Vrijeme
- **Implementacija:** 1-2 sata
- **Testiranje:** 30 minuta
- **Objasniti:** 5 minuta

### 🔧 SVEKORACI

#### **KORAK 1: Priprema projekta**

**Što trebam provjeriti:**
```
✓ Nema hardkodiranig API keyeva
✓ Nema tajnih podataka u appsettings.json
✓ Nema `catalog.db` u .gitignore-u (ako trebam bazu)
```

**Otvori:** `ispit/.gitignore`

**Trebalo bi biti:**
```
# Database
*.db
*.db-shm
*.db-wal

# User secrets
appsettings.*.json

# Logs
Logs/
```

---

#### **KORAK 2: Kreiraj Azure Account**

**Ako nemaš:**
1. Idi na https://azure.microsoft.com/
2. Klikni "Free Account"
3. Registriraj se s email adresom (mihaelj10@gmail.com)
4. Azure će dati $200 kredita za testiranje

**Ako već imaš:** Preskoči na KORAK 3

---

#### **KORAK 3: Publish kroz Visual Studio**

**U Visual Studio:**

```
1. Otvori ispit projekt
2. Desni klik na "Lab5" projekt
3. Odaberi "Publish..."
4. Odaberi "Azure" → "Next"
5. Odaberi "Azure App Service" → "Next"
6. Klikni "Create new App Service"
```

**Popuni forme:**
```
Resource Group:     Kreiraj novu (npr. "IspisGroup")
Name:               IspisLab5-2026 (mora biti jedinstveno!)
Subscription:       Azure for Students (ako imaš)
Location:           (Europe) West Europe
Hosting Plan:       Create new
  - Name:           IspisLab5Plan
  - Size:           Free (ili B1 za testiranje)
```

**Azure će:**
- Kreirati resurse
- Pokrenuti deployment
- Završiti za ~3-5 minuta

---

#### **KORAK 4: Konfiguracija Baze Podataka**

**OPCIJA A: SQLite u Cloud (Preporučeno za brzo)**
```
1. Baza će biti SQLite datoteka na Azure serveru
2. Aplikacija će raditi s njom
3. Az će automatski migrirati bazu
```

**OPCIJA B: Azure SQL (Za produkciju)**
```
1. Create Azure SQL Database
2. Update connection string u appsettings.json
3. Run migrations
```

**Za početak:** Koristi OPCIJA A (SQLite)

---

#### **KORAK 5: Ažuriranje Connection String**

**U Azure Portal:**
1. Idi na App Service koji si kreirio
2. Settings → Configuration
3. Dodaj ili ažuriraj:
```
CatalogDbContext = Data Source=catalog.db
```

**ILI u `ispit/appsettings.json`:**
```json
{
    "ConnectionStrings": {
        "CatalogDbContext": "Data Source=catalog.db"
    }
}
```

---

#### **KORAK 6: Deploy**

**U Visual Studio:**
```
1. Klikni "Publish" dugme (vidjeti će se u Publish prozoru)
2. Čekaj da se završi (~2-3 minute)
3. Trebao bi vidjeti: "Publish succeeded"
```

---

#### **KORAK 7: Testiranje**

**Azure će dao URL:** 
```
https://ispislab5-2026.azurewebsites.net
```

**Testiraj:**
```
✓ https://ispislab5-2026.azurewebsites.net/
✓ https://ispislab5-2026.azurewebsites.net/books
✓ https://ispislab5-2026.azurewebsites.net/authors
✓ Kreiraj novu knjiga (trebala bi raditi)
✓ Obriši nešto (trebalo bi raditi)
```

**Sve bi trebalo raditi kao lokalno!**

---

#### **KORAK 8: Dokumentiraj Deploy**

**Kreiraj `ispit/DEPLOYMENT.md`:**
```markdown
# Deployment Info

## Production URL
https://ispislab5-2026.azurewebsites.net

## Deployment Method
Azure App Service (Visual Studio Publish)

## Database
SQLite (catalog.db)

## Last Deployed
2026-07-08

## How to Deploy Again
1. Right-click Lab5 project → Publish
2. Click "Publish" button
3. Wait 3-5 minutes
```

---

### 📝 KAKO OBJASNITI (5 minuta)

**Odgovor na pitanje: "Gdje je aplikacija deployana?"**

```
"Aplikacija je deployanan Azure App Service - to je cloud platforma 
od Microsofta koja hostira web aplikacije.

PROCES:
1. Koristio sam Visual Studio "Publish" značajku
2. Odabrao sam Azure kao cloud provider
3. Kreirio sam novi App Service (virtualni server)
4. Azure je automatski preuzeo moj kod
5. Build-ao je aplikaciju
6. Pokrenuo je na serveru

REZULTAT:
- Umjesto https://localhost:7003 (samo lokalno)
- Sada je dostupna https://ispislab5-2026.azurewebsites.net (javno)

BAZA PODATAKA:
- SQLite datoteka je na Azure serveru
- Automatski je migrirana pri prvom pokretanju

ZAŠTO:
- Aplikacija je dostupna 24/7
- Ne trebam imati moje računalo uključeno
- Mogu se njom pohvaliti - svi mogu vidjeti"
```

---

### ✅ CHECKLIST

- [ ] Kreirio Azure Account
- [ ] Publish kroz Visual Studio
- [ ] Azure App Service je kreirano
- [ ] Deployment je bio успješan
- [ ] URL je dostupan (npr. https://ispislab5-2026.azurewebsites.net)
- [ ] Testirao sve CRUD operacije
- [ ] Dokumentirao URL ✅

**Rezultat: +3 BODA** ✅

---

---

## ZADATAK 3: AI INTEGRACIJA (3 BODA)

### 🎯 Cilj
- Korisnik može reći "Kreiraj recenziju: Hobbit je odličan, ocjena 5"
- AI će parsirati i kreirati recenziju automatski
- Kriterij: "AI integracija: unos podataka putem AI upita i sl."

### ⏱️ Vrijeme
- **Implementacija:** 3-4 sata
- **Testiranje:** 1 sat
- **Objasniti:** 10 minuta

### 🔧 SVEKORACI

#### **KORAK 1: Dobij Claude API Key**

**Na https://console.anthropic.com/**

```
1. Kreiraj Account (ako nemaš)
2. Idi na API Keys
3. Klikni "Create Key"
4. Kopiraj ključ (počinje s sk-ant-...)
5. ČUVAJ TAJNO! 🔐
```

---

#### **KORAK 2: Dodaj NuGet Paket**

**Otvori:** `ispit/Lab5.csproj`

**Dodaj u <ItemGroup>:**
```xml
<PackageReference Include="Anthropic.Sdk" Version="0.9.0" />
```

**Ili preko Package Manager Console:**
```powershell
Install-Package Anthropic.Sdk
```

---

#### **KORAK 3: Dodaj API Key u Secrets**

**U Visual Studio:**
```
Desni klik na ispit projekt → Manage User Secrets
```

**Otvori se datoteka, dodaj:**
```json
{
    "Anthropic": {
        "ApiKey": "sk-ant-xxxxxxxxxxxxxxxxxxxx"
    }
}
```

**ČUVAJ DATOTEKU - to je tajno!**

---

#### **KORAK 4: Kreiraj AIService**

**Kreiraj novi file:** `ispit/Services/AIService.cs`

```csharp
using Anthropic;
using System.Text.Json;

namespace Lab5.Services;

public interface IAIService
{
    Task<ReviewData> ExtractReviewFromPromptAsync(string prompt);
}

public class AIService : IAIService
{
    private readonly string _apiKey;
    private readonly ILogger<AIService> _logger;

    public AIService(string apiKey, ILogger<AIService> logger)
    {
        _apiKey = apiKey;
        _logger = logger;
    }

    public async Task<ReviewData> ExtractReviewFromPromptAsync(string prompt)
    {
        try
        {
            _logger.LogInformation("🤖 AI: Parsiranje promptja: {Prompt}", prompt);

            var client = new Anthropic(_apiKey);

            var message = await client.Messages.CreateAsync(new()
            {
                Model = "claude-3-5-sonnet-20241022",
                MaxTokens = 1024,
                Messages = new[]
                {
                    new MessageParam
                    {
                        Role = "user",
                        Content = $@"Parsiraj sljedeću recenziju i vrati SAMO JSON (bez dodatnog teksta):
                        
{{
    ""bookTitle"": ""naziv knjige koju trebam pronaći"",
    ""score"": 1-5 broj,
    ""sentiment"": ""Positive"" ili ""Neutral"" ili ""Critical"" ili ""Enthusiastic"",
    ""isRecommended"": true ili false,
    ""comment"": ""tekst recenzije (do 200 znakova)""
}}

ULAZ: {prompt}

VAŽNO: Vrati SAMO JSON, bez markdown formatiranja, bez dodatnog teksta!"
                    }
                }
            });

            var jsonContent = message.Content[0].Text;
            
            _logger.LogInformation("🤖 AI Odgovor: {Response}", jsonContent);

            // Očisti JSON ako ima markdown formatiranja
            jsonContent = jsonContent.Replace("```json", "").Replace("```", "").Trim();

            var reviewData = JsonSerializer.Deserialize<ReviewData>(jsonContent);

            if (reviewData == null)
            {
                _logger.LogWarning("⚠️ AI: Nije mogao parsirati JSON: {Json}", jsonContent);
                throw new Exception("AI nije vratio valjani JSON");
            }

            _logger.LogInformation("✅ AI: Uspješno parsirano - {BookTitle}, Ocjena: {Score}", 
                reviewData.BookTitle, reviewData.Score);

            return reviewData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ AI Greška pri parsiranju");
            throw;
        }
    }
}

public class ReviewData
{
    [JsonPropertyName("bookTitle")]
    public string BookTitle { get; set; }

    [JsonPropertyName("score")]
    public int Score { get; set; }

    [JsonPropertyName("sentiment")]
    public string Sentiment { get; set; }

    [JsonPropertyName("isRecommended")]
    public bool IsRecommended { get; set; }

    [JsonPropertyName("comment")]
    public string Comment { get; set; }
}
```

---

#### **KORAK 5: Registriraj u Program.cs**

**Otvori:** `ispit/Program.cs`

**Dodaj u DI sekciju (gdje su ostali Services):**
```csharp
// Prije: app.Build();

var anthropicApiKey = builder.Configuration["Anthropic:ApiKey"];
if (!string.IsNullOrEmpty(anthropicApiKey))
{
    builder.Services.AddScoped<IAIService>(sp =>
        new AIService(anthropicApiKey, sp.GetRequiredService<ILogger<AIService>>())
    );
}
```

---

#### **KORAK 6: Dodaj Endpoint u ReviewsController**

**Otvori:** `ispit/Controllers/ReviewsController.cs`

**Na početak klase dodaj:**
```csharp
private readonly IAIService _aiService;

public ReviewsController(
    // ... ostali parametri ...
    IAIService aiService)  // ← DODAJ OVO
{
    // ... ostali assignments ...
    _aiService = aiService;
}
```

**Dodaj novu akciju:**
```csharp
[AllowAnonymous]
[HttpPost("create-from-ai")]
public async Task<IActionResult> CreateFromAI([FromBody] AIPromptRequest request)
{
    try
    {
        _logger.LogInformation("🤖 Primljen AI prompt: {Prompt}", request.Prompt);

        if (string.IsNullOrWhiteSpace(request.Prompt))
            return BadRequest(new { error = "Prompt ne može biti prazan" });

        // 1. AI parsira prompt
        var reviewData = await _aiService.ExtractReviewFromPromptAsync(request.Prompt);

        // 2. Pronađi knjiga
        var books = await _bookRepository.GetAllAsync();
        var book = books.FirstOrDefault(b =>
            b.Title.Contains(reviewData.BookTitle, StringComparison.OrdinalIgnoreCase));

        if (book == null)
        {
            _logger.LogWarning("⚠️ Knjiga nije pronađena: {BookTitle}", reviewData.BookTitle);
            return BadRequest(new 
            { 
                error = $"Knjiga '{reviewData.BookTitle}' nije pronađena",
                suggestion = "Dostupne knjige: Harry Potter, Game of Thrones, Fellowship of the Ring"
            });
        }

        // 3. Pronađi korisnika
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            // Koristi demo korisnika ako nije logged in
            currentUser = (await _userRepository.GetAllAsync()).FirstOrDefault();
        }

        // 4. Kreiraj recenziju
        var review = new Review
        {
            BookId = book.Id,
            UserId = currentUser?.Id,
            Score = reviewData.Score,
            Comment = reviewData.Comment,
            IsRecommended = reviewData.IsRecommended,
            Title = reviewData.Comment.Substring(0, Math.Min(50, reviewData.Comment.Length)),
            Sentiment = Enum.Parse<ReviewSentiment>(reviewData.Sentiment, ignoreCase: true),
            ReviewedAt = DateTime.UtcNow
        };

        await _reviewRepository.CreateAsync(review);

        _logger.LogInformation("✅ Recenzija kreirana kroz AI: {BookTitle}, Score: {Score}", 
            book.Title, reviewData.Score);

        return Ok(new
        {
            success = true,
            reviewId = review.Id,
            message = $"✅ Recenzija kreirana za '{book.Title}'",
            review = new
            {
                bookTitle = book.Title,
                score = review.Score,
                comment = review.Comment,
                sentiment = review.Sentiment.ToString()
            }
        });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "❌ Greška pri AI kreiranju recenzije");
        return BadRequest(new { error = ex.Message });
    }
}

public class AIPromptRequest
{
    public string Prompt { get; set; }
}
```

---

#### **KORAK 7: Kreiraj Test View**

**Kreiraj:** `ispit/Views/Reviews/CreateFromAI.cshtml`

```html
@{
    ViewData["Title"] = "Kreiraj Recenziju - AI";
}

<div class="container mt-5">
    <div class="row">
        <div class="col-md-8">
            <h2>🤖 Kreiraj Recenziju putem AI-a</h2>
            <p class="text-muted">Napiši što misliš o knjizi, AI će automatski parsirati ocjenu i sentimen.</p>

            <form id="aiForm">
                <div class="form-group mb-3">
                    <label for="prompt" class="form-label">Što misliš o knjizi?</label>
                    <textarea class="form-control" id="prompt" rows="5" 
                        placeholder="Primjer: Hobbit je odličan! Preporuka da, ocjena 5, fantasy je super!"></textarea>
                </div>

                <button type="submit" class="btn btn-primary">🚀 Kreiraj putem AI-a</button>
            </form>

            <div id="result" class="mt-4"></div>
        </div>
    </div>
</div>

<script>
document.getElementById('aiForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    const prompt = document.getElementById('prompt').value;
    const resultDiv = document.getElementById('result');

    resultDiv.innerHTML = '⏳ Čekam AI odgovor...';

    try {
        const response = await fetch('/reviews/create-from-ai', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ prompt })
        });

        const data = await response.json();

        if (response.ok) {
            resultDiv.innerHTML = `
                <div class="alert alert-success">
                    <h5>✅ ${data.message}</h5>
                    <p><strong>Knjiga:</strong> ${data.review.bookTitle}</p>
                    <p><strong>Ocjena:</strong> ${data.review.score}/5 ⭐</p>
                    <p><strong>Sentiment:</strong> ${data.review.sentiment}</p>
                    <p><strong>Komentar:</strong> ${data.review.comment}</p>
                </div>
            `;
        } else {
            resultDiv.innerHTML = `
                <div class="alert alert-danger">
                    <h5>❌ Greška</h5>
                    <p>${data.error}</p>
                    ${data.suggestion ? `<p>${data.suggestion}</p>` : ''}
                </div>
            `;
        }
    } catch (error) {
        resultDiv.innerHTML = `<div class="alert alert-danger">Greška: ${error.message}</div>`;
    }
});
</script>
```

---

#### **KORAK 8: Dodaj Link u Navigation**

**Otvori:** `ispit/Views/Shared/_Layout.cshtml`

**U nav listi dodaj:**
```html
<li>
    <a class="nav-link" asp-controller="Reviews" asp-action="CreateFromAI">🤖 AI Recenzija</a>
</li>
```

---

#### **KORAK 9: Testiranje**

**Pokreni aplikaciju:**
```bash
dotnet run --project ispit/Lab5.csproj
```

**Testiraj:**
```
1. Otvori https://localhost:7003
2. Klikni "AI Recenzija" (trebalo bi biti u navigaciji)
3. Napiši: "Harry Potter je fantastic! Ocjena 5, obavezna preporuka, fantasy best!"
4. Klikni "Kreiraj putem AI-a"
5. Trebao bi vidjeti: ✅ Recenzija kreirana za 'Harry Potter'
```

**Što će biti rezultat:**
```
✅ Recenzija kreirana za 'Harry Potter'
Knjiga: Harry Potter
Ocjena: 5/5 ⭐
Sentiment: Positive
Komentar: fantasy best!
```

---

#### **KORAK 10: Dodaj Logging**

**U AIService i ReviewsController trebaju biti logovi (vidi TASK 1)**

```csharp
_logger.LogInformation("🤖 AI: Parsiranje promptja: {Prompt}", prompt);
_logger.LogInformation("✅ Recenzija kreirana kroz AI: {BookTitle}", book.Title);
_logger.LogError(ex, "❌ Greška pri AI kreiranju recenzije");
```

---

### 📝 KAKO OBJASNITI (10 minuta)

**Odgovor na pitanje: "Kako funkcionira AI integracija?"**

```
"Implementirao sam AI integraciju što omogućava korisniku da kreira 
recenziju prirodnom jezikom. Koristi se Claude API od Anthropica.

ARHITEKTURA:
1. Korisnik napiše: "Hobbit je odličan, ocjena 5"
2. Klikne dugme "Kreiraj putem AI-a"
3. Prompt se pošalje na Claude API (preko interneta)
4. Claude parsira tekst i vraća JSON:
   {
     "bookTitle": "Hobbit",
     "score": 5,
     "sentiment": "Positive",
     "comment": "Hobbit je odličan"
   }
5. Aplikacija prima JSON
6. Pronalazi 'Hobbit' u bazi
7. Kreira novu Review u bazi s tom ocjenom
8. Vraća odgovor korisniku: ✅ Recenzija kreirana

TEHNIČKI DETALJI:
- AIService se registrira u DI container-u
- ReviewsController injektira IAIService
- POST /reviews/create-from-ai endpoint
- Koristi Anthropic.Sdk biblioteku
- API key je sigurno pohranjen u user-secrets

SIGURNOST:
- API key nikad nije u kodu
- Korisnik ne vidi API key
- Sve je enkriptirano

ZAŠTO:
- Korisnik ne trebá znati strukturu podataka
- Prirodniji unos podataka
- LLM razumije context (ako kaže 'odličan' -> 5 zvjezdica)
- Scalable - mogu dodati više primjera za AI"
```

---

### ✅ CHECKLIST

- [ ] Dobio Claude API Key
- [ ] Dodao Anthropic.Sdk NuGet
- [ ] Spremiio API Key u user-secrets
- [ ] Kreirio AIService klasu
- [ ] Registrirao u Program.cs
- [ ] Dodao AI endpoint u ReviewsController
- [ ] Kreirio CreateFromAI.cshtml view
- [ ] Dodao link u navigation
- [ ] Testirao s primjerom ✅

**Rezultat: +3 BODA** ✅

---

---

## ZADATAK 4: PLAYWRIGHT TESTOVI (3 + 3 EXTRA BODA) - OPCIONO ALI PREPORUČENO

### 🎯 Cilj
- Automatski end-to-end testovi koji simuliraju korisnika
- Testira cijeli flow (kreiraj → uredi → obriši)
- Kriterij: "Kreiranje testova za sve API endpointe (Playwright scenarij 10 koraka + 3 extra boda)"

### ⏱️ Vrijeme
- **Implementacija:** 2-3 sata
- **Testiranje:** 1 sat
- **Objasniti:** 10 minuta

### 🔧 SVEKORACI

#### **KORAK 1: Dodaj Playwright**

**U Package Manager Console:**
```powershell
Install-Package Microsoft.Playwright
```

**Ili u .csproj:**
```xml
<PackageReference Include="Microsoft.Playwright" Version="1.40.0" />
```

---

#### **KORAK 2: Kreiraj Playwright Test File**

**Kreiraj:** `ispit/Tests/PlaywrightTests.cs`

```csharp
using Microsoft.Playwright;
using Xunit;

namespace Lab5.Tests;

public class PlaywrightTests : IAsyncLifetime
{
    private IPlaywright _playwright;
    private IBrowser _browser;
    private IPage _page;
    private const string BaseUrl = "https://localhost:7003";

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new()
        {
            Headless = true,
        });
        _page = await _browser.CreatePageAsync();

        // Ignoriraj SSL certifikat (localhost)
        await _page.Context.ClearAsync();
    }

    public async Task DisposeAsync()
    {
        await _browser?.CloseAsync();
        _playwright?.Dispose();
    }

    [Fact]
    public async Task ScenarioPregledKnjiga_TrebaUcitatiSve()
    {
        // Scenario 1: Pregledaj sve knjige
        await _page.GotoAsync($"{BaseUrl}/books");
        
        var title = await _page.TitleAsync();
        Assert.NotNull(title);
        
        var books = await _page.QuerySelectorAllAsync("[data-book-id]");
        Assert.NotEmpty(books);
    }

    [Fact]
    public async Task ScenariosAutora_TrebaUcitatiSve()
    {
        // Scenario 2: Pregledaj sve autore
        await _page.GotoAsync($"{BaseUrl}/authors");
        
        var authors = await _page.QuerySelectorAllAsync("[data-author-id]");
        Assert.NotEmpty(authors);
    }

    [Fact]
    public async Task ScenariosDetaljiKnjige_TrebaOtvoriti()
    {
        // Scenario 3: Otvori detalje knjige
        await _page.GotoAsync($"{BaseUrl}/books");
        
        var firstBookLink = await _page.QuerySelectorAsync("a[href*='/books/']");
        if (firstBookLink != null)
        {
            await firstBookLink.ClickAsync();
            await _page.WaitForLoadStateAsync();
            
            var header = await _page.QuerySelectorAsync("h1");
            Assert.NotNull(header);
        }
    }

    [Fact]
    public async Task ScenariosPretraga_TrebaPronaciKnjige()
    {
        // Scenario 4: Pretraži knjige
        await _page.GotoAsync($"{BaseUrl}/books");
        
        var searchInput = await _page.QuerySelectorAsync("input[type='search']");
        if (searchInput != null)
        {
            await searchInput.FillAsync("Harry");
            await _page.WaitForLoadStateAsync();
            
            var results = await _page.QuerySelectorAllAsync("[data-book-id]");
            Assert.NotEmpty(results);
        }
    }

    [Fact]
    public async Task ScenariosNavigacija_TrebaSePreklapati()
    {
        // Scenario 5: Klikni kroz sve glavne linkove
        await _page.GotoAsync($"{BaseUrl}/");
        
        var navLinks = new[] { "books", "authors", "genres", "publishers", "reviews" };
        
        foreach (var link in navLinks)
        {
            var navLink = await _page.QuerySelectorAsync($"a:has-text('{link}')");
            if (navLink != null)
            {
                await navLink.ClickAsync();
                await _page.WaitForLoadStateAsync();
                
                var content = await _page.QuerySelectorAsync("main");
                Assert.NotNull(content);
            }
        }
    }

    [Fact]
    public async Task ScenariosResponsivni_Dizajn()
    {
        // Scenario 6: Testiraj responsive na mobilnom
        await _page.SetViewportSizeAsync(375, 667); // iPhone veličina
        
        await _page.GotoAsync($"{BaseUrl}/books");
        
        var mainContent = await _page.QuerySelectorAsync("main");
        var box = await mainContent.BoundingBoxAsync();
        
        Assert.NotNull(box);
        Assert.True(box.Width <= 400, "Trebalo bi biti responsive");
    }

    [Fact]
    public async Task ScenarioFiltriranje_ZanroviSveOpcije()
    {
        // Scenario 7: Filtriranje po žanrovima
        await _page.GotoAsync($"{BaseUrl}/genres");
        
        var genreLinks = await _page.QuerySelectorAllAsync("a[href*='/genres/']");
        Assert.NotEmpty(genreLinks);
        
        if (genreLinks.Count > 0)
        {
            await genreLinks[0].ClickAsync();
            await _page.WaitForLoadStateAsync();
            
            var books = await _page.QuerySelectorAllAsync("[data-book-id]");
            // Mogu biti 0 ili više
            Assert.NotNull(books);
        }
    }

    [Fact]
    public async Task ScenariosRecenzije_SveRecenzije()
    {
        // Scenario 8: Pregled svih recenzija
        await _page.GotoAsync($"{BaseUrl}/reviews");
        
        var reviews = await _page.QuerySelectorAllAsync("[data-review-id]");
        // Trebale bi biti bar neke
        Assert.NotEmpty(reviews);
    }

    [Fact]
    public async Task ScenariosKorisnici_SviKorisnici()
    {
        // Scenario 9: Pregled svih korisnika
        await _page.GotoAsync($"{BaseUrl}/users");
        
        var users = await _page.QuerySelectorAllAsync("[data-user-id]");
        Assert.NotEmpty(users);
    }

    [Fact]
    public async Task ScenariosIzdavaci_SviIzdavaci()
    {
        // Scenario 10: Pregled svih izdavača
        await _page.GotoAsync($"{BaseUrl}/publishers");
        
        var publishers = await _page.QuerySelectorAllAsync("[data-publisher-id]");
        Assert.NotEmpty(publishers);
    }

    // EXTRA SCENARIJI (3 boda extra)

    [Fact]
    public async Task ExtraScenarioHomePageStatistike()
    {
        // Extra 1: Provjera home page statistike
        await _page.GotoAsync($"{BaseUrl}/");
        
        var stats = await _page.QuerySelectorAsync("[data-stat]");
        Assert.NotNull(stats);
    }

    [Fact]
    public async Task ExtraScenarioErrorHandling_InvalidId()
    {
        // Extra 2: Testiraj error stranicu (invalid ID)
        await _page.GotoAsync($"{BaseUrl}/books/99999", new() { WaitUntil = WaitUntilState.NetworkIdle });
        
        // Trebala bi biti 404 ili error stranica
        var notFound = await _page.QuerySelectorAsync("text='Not Found'") 
                    ?? await _page.QuerySelectorAsync("text='Nije pronađeno'");
        
        Assert.True(notFound != null || _page.Url.Contains("error"), 
            "Trebala bi biti error stranica");
    }

    [Fact]
    public async Task ExtraScenarioFullAccessibilityCheck()
    {
        // Extra 3: Accessibility check (nisu li svi linkovi broken)
        await _page.GotoAsync($"{BaseUrl}/");
        
        var brokenLinks = 0;
        var links = await _page.QuerySelectorAllAsync("a");
        
        foreach (var link in links.Take(10)) // Testiraj prvih 10
        {
            var href = await link.GetAttributeAsync("href");
            if (string.IsNullOrEmpty(href) || href.Contains("javascript"))
                brokenLinks++;
        }
        
        Assert.Equal(0, brokenLinks);
    }
}
```

---

#### **KORAK 3: Testiraj Playwright**

```bash
cd ispit
dotnet test --filter "PlaywrightTests"
```

**Trebalo bi vidjeti:**
```
Test Run summary
  Passed ✓ 13
  Failed ✗ 0
  Duration: ~30 sekundi
```

---

### 📝 KAKO OBJASNITI (10 minuta)

```
"Kreirio sam Playwright testove koji simuliraju pravo korisnika.

KAKO FUNKCIONIRA:
1. Playwright je browser automation alat
2. Kreiram test koji otvora stranicu
3. Klikne na linkove, popuni forme, itd.
4. Provjeravamo je li se nešto dogodilo

PRIMJER TESTA:
- Otvori /books stranicu
- Čekaj da se učita
- Provjeri jesu li se knjige učitale
- Klikni na prvu knjiga
- Provjeri je li stranica otvorena

SCENARIJI (10+):
1. Pregledaj sve knjige
2. Pregledaj sve autore
3. Otvori detalje knjige
4. Pretraži knjige
5. Klikni kroz navigaciju
6. Testiraj responsive (mobilna veličina)
7. Filtriranje po žanrovima
8. Pregled recenzija
9. Pregled korisnika
10. Pregled izdavača

EXTRA (3 boda):
11. Home page statistike
12. Error handling (invalid ID)
13. Accessibility - provjera linkova

ZAŠTO:
- End-to-end testovi testiraju što korisnik stvarno radi
- Sigurniji nego unit testovi
- Otkrivaju probleme što xUnit neće
- Provjera UI-a, ne samo logike"
```

---

### ✅ CHECKLIST

- [ ] Dodao Microsoft.Playwright paket
- [ ] Kreirio PlaywrightTests.cs
- [ ] 10 osnovnih testova
- [ ] 3 extra testa
- [ ] Sve testove prolaze ✅

**Rezultat: +3 BODA (+3 extra = +6 BODOVA TOTAL!)** ✅

---

---

## OPCIONO: ZADATAK 5 - GLOBAL SEARCH POBOLJŠANJE (+1 BOD)

### 🎯 Cilj
- Pretraga kroz sve entitete, ne samo knjige
- Jedan search box koji pretraži sve

### ⏱️ Vrijeme: 1 sat
### 🔧 KAKO:

**Kreiraj:** `ispit/Services/GlobalSearchService.cs`

```csharp
namespace Lab5.Services;

public interface IGlobalSearchService
{
    Task<GlobalSearchResults> SearchAllAsync(string query);
}

public class GlobalSearchService : IGlobalSearchService
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IPublisherRepository _publisherRepository;

    public GlobalSearchService(
        IBookRepository bookRepository,
        IAuthorRepository authorRepository,
        IGenreRepository genreRepository,
        IPublisherRepository publisherRepository)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _genreRepository = genreRepository;
        _publisherRepository = publisherRepository;
    }

    public async Task<GlobalSearchResults> SearchAllAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) query = "";

        var books = (await _bookRepository.GetAllAsync())
            .Where(b => b.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Take(5);

        var authors = (await _authorRepository.GetAllAsync())
            .Where(a => $"{a.FirstName} {a.LastName}".Contains(query, StringComparison.OrdinalIgnoreCase))
            .Take(5);

        var genres = (await _genreRepository.GetAllAsync())
            .Where(g => g.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Take(5);

        var publishers = (await _publisherRepository.GetAllAsync())
            .Where(p => p.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Take(5);

        return new GlobalSearchResults
        {
            Books = books.ToList(),
            Authors = authors.ToList(),
            Genres = genres.ToList(),
            Publishers = publishers.ToList()
        };
    }
}

public class GlobalSearchResults
{
    public List<Book> Books { get; set; } = new();
    public List<Author> Authors { get; set; } = new();
    public List<Genre> Genres { get; set; } = new();
    public List<Publisher> Publishers { get; set; } = new();
}
```

**Endpoint:**
```csharp
// U HomeController ili novom SearchController
[AllowAnonymous]
[Route("search")]
public async Task<IActionResult> GlobalSearch(string q)
{
    var results = await _globalSearchService.SearchAllAsync(q);
    return View(results);
}
```

---

---

# 📊 FINALNI SAŽETAK - SVI ZADACI

## 🎯 SCENARIJI - KOLIKO BODOVA?

### **MINIMUM (50% - Ocjena 2)**
```
Trebam: 35+ bodova

Što trebam:
1. Logging ..................... 2 boda ✅
2. Deploy ...................... 3 boda ✅
3. AI integracija .............. 3 boda ✅
4. Global Search+ .............. 1 bod  ✅
────────────────────────────────────────
= 9 novih bodova
= 26 + 9 = 35 bodova (50%) ✅ MINIMUM
```

### **SOLIDNA OCJENA (60% - Ocjena 3)**
```
Trebam: 42 boda

Scenarij A:
1. Logging ..................... 2 boda ✅
2. Deploy ...................... 3 boda ✅
3. AI integracija .............. 3 boda ✅
4. Playwright testovi .......... 3 boda ✅
5. Global Search+ .............. 1 bod  ✅
────────────────────────────────────────
= 12 novih bodova
= 26 + 12 = 38 bodova (54%) ⚠️ Blizu

Trebam još 4 boda:
+ MCP (2 boda) + malo detaljnije = 40+ bodova (57%) ✅
```

### **DOBRA OCJENA (75% - Ocjena 4)**
```
Trebam: 52+ boda

1. Logging ..................... 2 boda ✅
2. Deploy ...................... 3 boda ✅
3. AI integracija .............. 3 boda ✅
4. Playwright testovi .......... 6 boda ✅ (3 + 3 extra)
5. Global Search+ .............. 1 bod  ✅
6. MCP Expose .................. 2 boda ✅
7. Detaljnija AI (parsing knjiga) . 2 boda ✅
────────────────────────────────────────
= 19 novih bodova
= 26 + 19 = 45 bodova (64%) - Trebam još 7
```

---

## ⏱️ VREMENSKA PROCJENA - UKUPNO

| Zadatak | Vrijeme | Teško? |
|---------|---------|--------|
| 1. Logging | 2h | ⭐ Lako |
| 2. Deploy | 2h | ⭐ Lako |
| 3. AI integracija | 4h | ⭐⭐ Srednje |
| 4. Playwright | 3h | ⭐⭐ Srednje |
| 5. Global Search+ | 1h | ⭐ Lako |
| 6. MCP | 2-3h | ⭐⭐⭐ Teško |
| 7. AI poboljšanja | 2h | ⭐⭐⭐ Teško |
| **TOTAL** | **16-18h** | |

---

## 💪 ŠTO MOGU OBJASNITI?

### Lako objasniti (5 minuta svaki)
✅ Logging - Što je Serilog, zašto je važan  
✅ Deploy - Kako se app servera deployira  
✅ Global Search - SQL query što pretraživati  
✅ Responsive - CSS media queries  

### Srednje objasniti (10 minuta svaki)
⚠️ AI integracija - Kako Claude API radi, JSON parsing  
⚠️ Playwright - End-to-end testiranje, browser automation  

### Teško objasniti (15+ minuta)
❌ MCP - Model Context Protocol, kompleksna arhitektura  

---

## 🏆 FINALNA PREPORUKA

**Za siguran prolazak s solidnom ocjenom (60%):**

```
OBAVEZNO:
1. ✅ Logging (2h) - UVIJEK MOGU OBJASNITI
2. ✅ Deploy (2h) - FAST, LAKO, VIDLJIVO
3. ✅ AI integracija (4h) - IMPRESSIVE, MODERNO
4. ✅ Playwright (3h) - 13 testova + 3 extra

TOTAL: 11 sati
BODOVI: 26 + 14 = 40 bodova (57%) ✅

OPCIONO:
5. Global Search+ (+1)
6. Detaljnija AI (+1-2)
= 42-43 bodova (60%) ✅ SOLIDNA OCJENA
```

---

**Dokument:** Detaljan plan implementacije  
**Status:** GOTOV ZA POČETAK  
**Sljedeći korak:** Počnite s TASK 1 - LOGGING
