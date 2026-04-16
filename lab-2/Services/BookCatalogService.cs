using Lab2.Models;
using Lab2.ViewModels;

namespace Lab2.Services;

public class BookCatalogService : IBookCatalogService
{
    private readonly CatalogMockStore _store;

    public BookCatalogService(CatalogMockStore store)
    {
        _store = store;
    }

    public Task<BookDashboardViewModel> GetDashboardAsync()
    {
        var books = _store.Books.ToList();
        var genres = _store.Genres.ToList();
        var reviews = _store.Reviews
            .OrderByDescending(review => review.ReviewedAt)
            .ToList();

        var topBooks = books
            .Select(BuildBookCard)
            .OrderByDescending(book => book.AverageRating)
            .ThenByDescending(book => book.ReviewCount)
            .ToList();

        var recentReviews = reviews
            .Take(5)
            .Select(review => new ReviewCardViewModel
            {
                BookTitle = review.Book.Title,
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
            TotalAuthors = _store.Authors.Count,
            TotalUsers = _store.Users.Count,
            OverallAverageRating = CalculateOverallAverageRating(reviews),
            TopBooks = topBooks.Take(3).ToList(),
            RecentReviews = recentReviews,
            GenreStats = genreStats
        };

        return Task.FromResult(dashboard);
    }

    private static BookCardViewModel BuildBookCard(Book book)
    {
        var reviewCount = book.Reviews.Count;
        var averageRating = reviewCount == 0 ? 0 : book.Reviews.Average(review => review.Score);

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
