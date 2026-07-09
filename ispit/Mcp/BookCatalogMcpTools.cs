using System.ComponentModel;
using Lab5.Services;
using ModelContextProtocol.Server;

namespace Lab5.Mcp;

/// <summary>
/// Izlaže katalog knjiga kao MCP alate dostupne kroz agentic IDE (npr. Claude Code, Cursor).
/// Endpoint: /mcp (HTTP transport)
/// </summary>
[McpServerToolType]
public class BookCatalogMcpTools
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IPublisherRepository _publisherRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IGlobalSearchService _searchService;

    public BookCatalogMcpTools(
        IBookRepository bookRepository,
        IAuthorRepository authorRepository,
        IGenreRepository genreRepository,
        IPublisherRepository publisherRepository,
        IReviewRepository reviewRepository,
        IGlobalSearchService searchService)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _genreRepository = genreRepository;
        _publisherRepository = publisherRepository;
        _reviewRepository = reviewRepository;
        _searchService = searchService;
    }

    [McpServerTool, Description("Pretraži cijeli katalog (knjige, autori, žanrovi, izdavači, recenzije, korisnici) po pojmu.")]
    public async Task<string> SearchCatalog([Description("Pojam za pretragu")] string query)
    {
        var results = await _searchService.SearchAllAsync(query);
        return $"Knjige: {string.Join(", ", results.Books.Select(b => b.Title))}\n" +
               $"Autori: {string.Join(", ", results.Authors.Select(a => $"{a.FirstName} {a.LastName}"))}\n" +
               $"Žanrovi: {string.Join(", ", results.Genres.Select(g => g.Name))}\n" +
               $"Izdavači: {string.Join(", ", results.Publishers.Select(p => p.Name))}\n" +
               $"Recenzije: {string.Join(", ", results.Reviews.Select(r => r.Title))}\n" +
               $"Korisnici: {string.Join(", ", results.Users.Select(u => u.FullName))}";
    }

    [McpServerTool, Description("Vrati popis svih knjiga u katalogu s osnovnim podacima (naslov, autor, izdavač, status).")]
    public async Task<string> ListBooks()
    {
        var books = await _bookRepository.GetAllAsync();
        return string.Join("\n", books.Select(b =>
            $"#{b.Id} \"{b.Title}\" — {b.Author?.FirstName} {b.Author?.LastName}, izdavač: {b.Publisher?.Name ?? "-"}, status: {b.Status}"));
    }

    [McpServerTool, Description("Vrati detalje jedne knjige po ID-u, uključujući prosječnu ocjenu i broj recenzija.")]
    public async Task<string> GetBookDetails([Description("ID knjige")] int bookId)
    {
        var book = await _bookRepository.GetByIdAsync(bookId);
        if (book is null)
        {
            return $"Knjiga s ID {bookId} nije pronađena.";
        }

        var averageScore = book.Reviews.Count == 0 ? 0 : book.Reviews.Average(r => r.Score);
        return $"\"{book.Title}\" (ISBN {book.Isbn})\n" +
               $"Autor: {book.Author?.FirstName} {book.Author?.LastName}\n" +
               $"Izdavač: {book.Publisher?.Name ?? "-"}\n" +
               $"Status: {book.Status}, {book.PageCount} stranica\n" +
               $"Prosječna ocjena: {averageScore:0.0} ({book.Reviews.Count} recenzija)";
    }

    [McpServerTool, Description("Vrati popis svih autora u katalogu.")]
    public async Task<string> ListAuthors()
    {
        var authors = await _authorRepository.GetAllAsync();
        return string.Join("\n", authors.Select(a =>
            $"#{a.Id} {a.FirstName} {a.LastName} ({a.Nationality}) — {a.Books.Count} knjiga"));
    }

    [McpServerTool, Description("Vrati popis svih žanrova u katalogu.")]
    public async Task<string> ListGenres()
    {
        var genres = await _genreRepository.GetAllAsync();
        return string.Join("\n", genres.Select(g => $"#{g.Id} {g.Name}"));
    }

    [McpServerTool, Description("Vrati popis svih izdavača u katalogu.")]
    public async Task<string> ListPublishers()
    {
        var publishers = await _publisherRepository.GetAllAsync();
        return string.Join("\n", publishers.Select(p => $"#{p.Id} {p.Name} ({p.City}, {p.Country})"));
    }

    [McpServerTool, Description("Vrati najnovije recenzije u katalogu, do zadanog broja rezultata (zadano 10).")]
    public async Task<string> ListRecentReviews([Description("Maksimalan broj recenzija za vratiti")] int limit = 10)
    {
        var reviews = await _reviewRepository.GetAllAsync();
        var recent = reviews.OrderByDescending(r => r.ReviewedAt).Take(limit);
        return string.Join("\n", recent.Select(r =>
            $"\"{r.Title}\" za \"{r.Book?.Title}\" — {r.Score}/5, {r.Sentiment}, autor: {r.User?.FullName}"));
    }
}
