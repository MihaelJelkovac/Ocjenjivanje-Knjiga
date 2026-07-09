using Lab5.Models;

namespace Lab5.Services;

public interface IGlobalSearchService
{
    Task<GlobalSearchResults> SearchAllAsync(string? query);
}

public class GlobalSearchResults
{
    public string Query { get; set; } = string.Empty;
    public List<Book> Books { get; set; } = new();
    public List<Author> Authors { get; set; } = new();
    public List<Genre> Genres { get; set; } = new();
    public List<Publisher> Publishers { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
    public List<User> Users { get; set; } = new();

    public int TotalCount => Books.Count + Authors.Count + Genres.Count + Publishers.Count + Reviews.Count + Users.Count;
}

public class GlobalSearchService : IGlobalSearchService
{
    private const int MaxResultsPerCategory = 10;

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
