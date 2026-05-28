using Lab5.Data;
using Lab5.Models;
using Lab5.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Lab5.Services;

public class BookCatalogService : IBookCatalogService
{
    private readonly CatalogDbContext _context;
    private readonly IMemoryCache _cache;
    private const string DashboardCacheKey = "dashboard_cache";
    private static readonly TimeSpan DashboardCacheDuration = TimeSpan.FromMinutes(10);

    public BookCatalogService(CatalogDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<BookDashboardViewModel> GetDashboardAsync()
    {
        // Try to get from cache first
        if (_cache.TryGetValue(DashboardCacheKey, out BookDashboardViewModel? cachedDashboard))
        {
            return cachedDashboard!;
        }

        // Load books without Reviews (will use separate Reviews query with User info)
        var books = await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Publisher)
            .Include(b => b.BookGenres)
            .ThenInclude(bg => bg.Genre)
            .ToListAsync();

        // Load all reviews with User and Book info for calculations and recent reviews panel
        var reviews = await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Book)
            .OrderByDescending(review => review.ReviewedAt)
            .ToListAsync();

        var genres = await _context.Genres
            .Include(g => g.BookGenres)
            .ToListAsync();

        // Create dictionary for O(1) review lookup by BookId
        var reviewsByBookId = reviews
            .GroupBy(r => r.BookId)
            .ToDictionary(g => g.Key, g => g.ToList());

        // Count authors and users directly from database (efficient Count queries)
        var authorsCount = await _context.Authors.CountAsync();
        var usersCount = await _context.Users.CountAsync();

        var topBooks = books
            .Select(b => BuildBookCard(b, reviewsByBookId.GetValueOrDefault(b.Id, new List<Review>())))
            .OrderByDescending(book => book.AverageRating)
            .ThenByDescending(book => book.ReviewCount)
            .ToList();

        var recentReviews = reviews
            .Take(5)
            .Select(review => new ReviewCardViewModel
            {
                BookTitle = review.Book?.Title ?? "-",
                ReviewerName = review.User.FullName,
                Score = review.Score,
                Sentiment = review.Sentiment,
                Comment = review.Comment,
                ReviewedAt = review.ReviewedAt
            })
            .ToList();

        var genreStats = genres
            .Select(genre => new GenreStatViewModel
            {
                GenreName = genre.Name,
                BookCount = genre.BookGenres.Count,
                AverageRating = CalculateGenreAverageRating(genre.Id, books, reviews)
            })
            .OrderByDescending(stat => stat.AverageRating)
            .ThenByDescending(stat => stat.BookCount)
            .ToList();

        var dashboard = new BookDashboardViewModel
        {
            TotalBooks = books.Count,
            TotalAuthors = authorsCount,
            TotalUsers = usersCount,
            OverallAverageRating = CalculateOverallAverageRating(reviews),
            TopBooks = topBooks.Take(3).ToList(),
            RecentReviews = recentReviews,
            GenreStats = genreStats
        };

        // Cache for 10 minutes with sliding expiration
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = DashboardCacheDuration,
            SlidingExpiration = TimeSpan.FromMinutes(5)
        };
        _cache.Set(DashboardCacheKey, dashboard, cacheOptions);

        return dashboard;
    }

    private static BookCardViewModel BuildBookCard(Book book, IReadOnlyList<Review> bookReviews)
    {
        var reviewCount = bookReviews.Count;
        var averageRating = reviewCount == 0 ? 0 : bookReviews.Average(review => review.Score);

        return new BookCardViewModel
        {
            Id = book.Id,
            Title = book.Title,
            AuthorName = $"{book.Author.FirstName} {book.Author.LastName}",
            PublisherName = book.Publisher.Name,
            Genres = string.Join(", ", book.BookGenres.Select(bookGenre => bookGenre.Genre.Name)),
            Status = book.Status,
            PublishedOn = book.PublishedOn,
            AverageRating = averageRating,
            ReviewCount = reviewCount
        };
    }

    private static double CalculateOverallAverageRating(IReadOnlyList<Review> reviews)
    {
        return reviews.Count == 0 ? 0 : reviews.Average(review => review.Score);
    }

    private static double CalculateGenreAverageRating(int genreId, IReadOnlyList<Book> books, IReadOnlyList<Review> reviews)
    {
        var genreBookIds = books
            .Where(book => book.BookGenres.Any(bookGenre => bookGenre.GenreId == genreId))
            .Select(book => book.Id)
            .ToHashSet();

        var genreRatings = reviews
            .Where(review => genreBookIds.Contains(review.BookId))
            .Select(review => review.Score)
            .ToList();

        return genreRatings.Count == 0 ? 0 : genreRatings.Average();
    }
}

