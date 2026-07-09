using Lab5.Models;

namespace Lab5.Services;

public interface IGlobalSearchService
{
    Task<GlobalSearchResults> SearchAllAsync(string? query);
}

public class GlobalSearchResults
{
    public string Query { get; set; } = string.Empty;
    public List<NavPageResult> Pages { get; set; } = new();
    public List<Book> Books { get; set; } = new();
    public List<Author> Authors { get; set; } = new();
    public List<Genre> Genres { get; set; } = new();
    public List<Publisher> Publishers { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
    public List<User> Users { get; set; } = new();

    public int TotalCount => Pages.Count + Books.Count + Authors.Count + Genres.Count + Publishers.Count + Reviews.Count + Users.Count;
}

/// <summary>
/// Stavka izbornika / stranica u aplikaciji (za pretragu "izbornika i stranica").
/// </summary>
public class NavPageResult
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
}

public class GlobalSearchService : IGlobalSearchService
{
    private const int MaxResultsPerCategory = 10;

    // Statične stranice / stavke izbornika koje se mogu pretraživati (uz podatke).
    private static readonly NavPageResult[] AppPages =
    {
        new() { Title = "Početna", Description = "Naslovna stranica s pregledom kataloga", Url = "/Home", Keywords = "pocetna home naslovna dashboard" },
        new() { Title = "Rating Arena", Description = "Usporedba i ocjenjivanje knjiga", Url = "/Home/arena", Keywords = "rating arena ocjene ocjenjivanje usporedba" },
        new() { Title = "Knjige", Description = "Katalog svih knjiga", Url = "/knjige", Keywords = "knjige books katalog naslovi" },
        new() { Title = "Autori", Description = "Popis svih autora", Url = "/autori", Keywords = "autori authors pisci" },
        new() { Title = "Žanrovi", Description = "Popis svih žanrova", Url = "/zanrovi", Keywords = "zanrovi genres kategorije" },
        new() { Title = "Izdavači", Description = "Popis svih izdavača", Url = "/izdavaci", Keywords = "izdavaci publishers nakladnici" },
        new() { Title = "Recenzije", Description = "Sve recenzije knjiga", Url = "/recenzije", Keywords = "recenzije reviews ocjene dojmovi" },
        new() { Title = "Korisnici", Description = "Popis korisnika", Url = "/korisnici", Keywords = "korisnici users clanovi" },
        new() { Title = "Pretraga", Description = "Globalna pretraga kroz cijeli katalog", Url = "/pretraga", Keywords = "pretraga search trazi" },
        new() { Title = "Privatnost", Description = "Pravila privatnosti", Url = "/Home/privacy", Keywords = "privatnost privacy pravila" },
    };

    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IPublisherRepository _publisherRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GlobalSearchService> _logger;

    public GlobalSearchService(
        IBookRepository bookRepository,
        IAuthorRepository authorRepository,
        IGenreRepository genreRepository,
        IPublisherRepository publisherRepository,
        IReviewRepository reviewRepository,
        IUserRepository userRepository,
        ILogger<GlobalSearchService> logger)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _genreRepository = genreRepository;
        _publisherRepository = publisherRepository;
        _reviewRepository = reviewRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<GlobalSearchResults> SearchAllAsync(string? query)
    {
        var normalizedQuery = query?.Trim() ?? string.Empty;
        _logger.LogInformation("🔍 Globalna pretraga: '{Query}'", normalizedQuery);

        var results = new GlobalSearchResults { Query = normalizedQuery };

        if (string.IsNullOrWhiteSpace(normalizedQuery))
        {
            return results;
        }

        // Pretraga izbornika / stranica (statične stavke, bez baze)
        results.Pages = AppPages
            .Where(p =>
                p.Title.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                p.Keywords.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var books = await _bookRepository.GetAllAsync();
        results.Books = books
            .Where(b =>
                b.Title.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                b.Isbn.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                (b.Author != null && $"{b.Author.FirstName} {b.Author.LastName}".Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase)) ||
                (b.Publisher != null && b.Publisher.Name.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase)))
            .Take(MaxResultsPerCategory)
            .ToList();

        var authors = await _authorRepository.GetAllAsync();
        results.Authors = authors
            .Where(a =>
                a.FirstName.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                a.LastName.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                a.Nationality.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase))
            .Take(MaxResultsPerCategory)
            .ToList();

        var genres = await _genreRepository.GetAllAsync();
        results.Genres = genres
            .Where(g =>
                g.Name.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                g.Description.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase))
            .Take(MaxResultsPerCategory)
            .ToList();

        var publishers = await _publisherRepository.GetAllAsync();
        results.Publishers = publishers
            .Where(p =>
                p.Name.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                p.City.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                p.Country.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase))
            .Take(MaxResultsPerCategory)
            .ToList();

        var reviews = await _reviewRepository.GetAllAsync();
        results.Reviews = reviews
            .Where(r =>
                r.Title.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                r.Comment.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                (r.Book != null && r.Book.Title.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase)))
            .Take(MaxResultsPerCategory)
            .ToList();

        var users = await _userRepository.GetAllAsync();
        results.Users = users
            .Where(u =>
                u.FullName.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                u.Username.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase))
            .Take(MaxResultsPerCategory)
            .ToList();

        _logger.LogInformation("✅ Globalna pretraga '{Query}' vratila {Count} rezultata", normalizedQuery, results.TotalCount);

        return results;
    }
}
