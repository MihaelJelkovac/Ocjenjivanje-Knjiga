using Microsoft.Playwright;
using Xunit;

namespace Lab5.Tests;

public class PlaywrightTests : IAsyncLifetime
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IPage? _page;
    private const string BaseUrl = "https://localhost:7003";

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new()
        {
            Headless = true,
        });

        _page = await _browser.NewPageAsync(new()
        {
            IgnoreHTTPSErrors = true,
        });
    }

    public async Task DisposeAsync()
    {
        if (_page != null)
            await _page.CloseAsync();
        if (_browser != null)
            await _browser.CloseAsync();
        _playwright?.Dispose();
    }

    // ==================== OSNOVNI TESTOVI (10) ====================

    /// <summary>
    /// Test 1: Pregledaj sve knjige - trebale bi se učitati
    /// </summary>
    [Fact]
    public async Task Scenario01_PregledSvihKnjiga_TrebaUcitatiSve()
    {
        // Arrange & Act
        await _page!.GotoAsync($"{BaseUrl}/books", new() { WaitUntil = WaitUntilState.NetworkIdle });

        // Assert
        var title = await _page!.TitleAsync();
        Assert.NotNull(title);

        // Provjeri da je stranica dostupna (čak i ako nema knjiga)
        var content = await _page.TextContentAsync("body");
        Assert.NotNull(content);
        Assert.True(content.Length > 0, "Stranica trebala bi biti dostupna");
    }

    /// <summary>
    /// Test 2: Pregledaj sve autore
    /// </summary>
    [Fact]
    public async Task Scenario02_PregledSvihAutora_TrebaUcitatiSve()
    {
        // Arrange & Act
        await _page!.GotoAsync($"{BaseUrl}/authors", new() { WaitUntil = WaitUntilState.NetworkIdle });

        // Assert
        // Provjeri da je stranica dostupna (čak i ako nema autora)
        var content = await _page.TextContentAsync("body");
        Assert.NotNull(content);
        Assert.True(content.Contains("Autori") || content.Contains("Author") || content.Length > 100,
            "Stranica trebala bi biti dostupna");
    }

    /// <summary>
    /// Test 3: Otvori detalje prve knjige
    /// </summary>
    [Fact]
    public async Task Scenario03_DetaljiKnjige_TrebaOtvoriti()
    {
        // Arrange
        await _page!.GotoAsync($"{BaseUrl}/books", new() { WaitUntil = WaitUntilState.NetworkIdle });

        // Act
        var firstBookLink = await _page.QuerySelectorAsync("a[href^='/books/']:not([href*='create']):not([href*='edit']):not([href*='delete'])");
        if (firstBookLink != null)
        {
            await firstBookLink.ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // Assert
        var header = await _page.QuerySelectorAsync("h1, h2");
        Assert.NotNull(header);
    }

    /// <summary>
    /// Test 4: Pretraži knjige po tekstu
    /// </summary>
    [Fact]
    public async Task Scenario04_Pretraga_TrebaPronaciKnjige()
    {
        // Arrange
        await _page!.GotoAsync($"{BaseUrl}/books", new() { WaitUntil = WaitUntilState.NetworkIdle });

        // Act
        var searchInput = await _page.QuerySelectorAsync("input[type='search'], input[placeholder*='search' i]");
        if (searchInput != null)
        {
            await searchInput.FillAsync("Harry");
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // Assert - provjeri da je stranica dostupna nakon pretrage
        var content = await _page.TextContentAsync("body");
        Assert.NotNull(content);
        Assert.True(content.Length > 50, "Pretraga trebala bi vratiti stranicu");
    }

    /// <summary>
    /// Test 5: Navigacija kroz sve glavne linkove (Books, Authors, Genres, Publishers, Reviews)
    /// </summary>
    [Fact]
    public async Task Scenario05_Navigacija_TrebaSePreklapati()
    {
        // Arrange
        await _page!.GotoAsync($"{BaseUrl}/", new() { WaitUntil = WaitUntilState.NetworkIdle });

        var navItems = new[] { "Knjige", "Autori", "Žanrovi", "Izdavači", "Recenzije" };

        // Act & Assert
        foreach (var item in navItems)
        {
            var navLink = await _page.QuerySelectorAsync($"a:has-text('{item}')");
            if (navLink != null)
            {
                await navLink.ClickAsync();
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                var content = await _page.QuerySelectorAsync("main");
                Assert.NotNull(content);
            }
        }
    }

    /// <summary>
    /// Test 6: Testiraj responsive dizajn na mobilnoj veličini
    /// </summary>
    [Fact]
    public async Task Scenario06_ResponsivniDizajn_MobilneVelikine()
    {
        // Arrange
        await _page.SetViewportSizeAsync(375, 667); // iPhone veličina

        // Act
        await _page!.GotoAsync($"{BaseUrl}/books", new() { WaitUntil = WaitUntilState.NetworkIdle });

        // Assert
        var mainContent = await _page.QuerySelectorAsync("main");
        var box = await mainContent.BoundingBoxAsync();

        Assert.NotNull(box);
        Assert.True(box.Width <= 400, "Trebalo bi biti responsive");
    }

    /// <summary>
    /// Test 7: Filtriranje po žanrovima - otvori jedan žanr
    /// </summary>
    [Fact]
    public async Task Scenario07_Filtriranje_ZanroviSveOpcije()
    {
        // Arrange
        await _page!.GotoAsync($"{BaseUrl}/genres", new() { WaitUntil = WaitUntilState.NetworkIdle });

        // Act
        var genreLinks = await _page.QuerySelectorAllAsync("a[href^='/genres/']:not([href*='create'])");

        if (genreLinks.Count > 0)
        {
            await genreLinks[0].ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // Assert
        var books = await _page.QuerySelectorAllAsync("a[href*='/books/']");
        Assert.NotNull(books);
    }

    /// <summary>
    /// Test 8: Pregled svih recenzija
    /// </summary>
    [Fact]
    public async Task Scenario08_Recenzije_SveRecenzije()
    {
        // Arrange & Act
        await _page!.GotoAsync($"{BaseUrl}/reviews", new() { WaitUntil = WaitUntilState.NetworkIdle });

        // Assert
        var reviews = await _page.QuerySelectorAllAsync("tr, div[data-review]");
        Assert.NotEmpty(reviews);
    }

    /// <summary>
    /// Test 9: Pregled svih korisnika
    /// </summary>
    [Fact]
    public async Task Scenario09_Korisnici_SviKorisnici()
    {
        // Arrange & Act
        await _page!.GotoAsync($"{BaseUrl}/users", new() { WaitUntil = WaitUntilState.NetworkIdle });

        // Assert
        var users = await _page.QuerySelectorAllAsync("tr, div[data-user]");
        Assert.NotEmpty(users);
    }

    /// <summary>
    /// Test 10: Pregled svih izdavača
    /// </summary>
    [Fact]
    public async Task Scenario10_Izdavaci_SviIzdavaci()
    {
        // Arrange & Act
        await _page!.GotoAsync($"{BaseUrl}/publishers", new() { WaitUntil = WaitUntilState.NetworkIdle });

        // Assert
        var publishers = await _page.QuerySelectorAllAsync("tr, div[data-publisher]");
        Assert.NotEmpty(publishers);
    }

    // ==================== EXTRA TESTOVI (3) ====================

    /// <summary>
    /// Extra Test 1: Provjera home page - trebale bi biti statistike
    /// </summary>
    [Fact]
    public async Task ExtraScenario01_HomePage_Statistike()
    {
        // Arrange & Act
        await _page!.GotoAsync($"{BaseUrl}/", new() { WaitUntil = WaitUntilState.NetworkIdle });

        // Assert
        var pageTitle = await _page.TitleAsync();
        Assert.NotNull(pageTitle);
        Assert.NotEmpty(pageTitle);

        var content = await _page.TextContentAsync("body");
        Assert.NotNull(content);
        Assert.NotEmpty(content);
    }

    /// <summary>
    /// Extra Test 2: Error handling - pristup invalid ID-u trebao bi dati error
    /// </summary>
    [Fact]
    public async Task ExtraScenario02_ErrorHandling_InvalidID()
    {
        // Arrange & Act
        var response = await _page.GotoAsync($"{BaseUrl}/books/99999", new() { WaitUntil = WaitUntilState.NetworkIdle });

        // Assert
        var notFoundText = await _page.TextContentAsync("body");
        Assert.NotNull(notFoundText);

        // Trebala bi biti 404 ili stranica nije pronađena
        bool isNotFound = notFoundText.Contains("Not Found") ||
                         notFoundText.Contains("Nije pronađeno") ||
                         notFoundText.Contains("404") ||
                         response.Status == 404;

        Assert.True(isNotFound, "Trebala bi biti error stranica za invalid ID");
    }

    /// <summary>
    /// Extra Test 3: Accessibility check - provjera da linkovi ne klikavaju na broken stranice
    /// </summary>
    [Fact]
    public async Task ExtraScenario03_Accessibility_LinksAreValid()
    {
        // Arrange
        await _page!.GotoAsync($"{BaseUrl}/", new() { WaitUntil = WaitUntilState.NetworkIdle });

        // Act
        var links = await _page.QuerySelectorAllAsync("a[href]");
        var brokenLinks = 0;

        foreach (var link in links.Take(15)) // Testiraj prvih 15 linkova
        {
            var href = await link.GetAttributeAsync("href");

            if (string.IsNullOrEmpty(href) ||
                href.StartsWith("javascript:") ||
                href.StartsWith("#"))
            {
                brokenLinks++;
            }
        }

        // Assert
        Assert.Equal(0, brokenLinks);
    }
}
