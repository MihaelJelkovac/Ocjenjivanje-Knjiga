using Lab1.Models;
using Lab1.ViewModels;

namespace Lab1.Services;

public class BookCatalogService : IBookCatalogService
{
    private readonly List<Author> _authors;
    private readonly List<Publisher> _publishers;
    private readonly List<Genre> _genres;
    private readonly List<User> _users;
    private readonly List<Book> _books;
    private readonly List<BookGenre> _bookGenres;
    private readonly List<Review> _reviews;

    public BookCatalogService()
    {
        (_authors, _publishers, _genres, _users, _books, _bookGenres, _reviews) = SeedData();
    }

    public async Task<BookDashboardViewModel> GetDashboardAsync()
    {
        await Task.Delay(120);

        var topBooks = _books
            .Select(BuildBookCard)
            .OrderByDescending(book => book.AverageRating)
            .ThenByDescending(book => book.ReviewCount)
            .ToList();

        var recentReviews = _reviews
            .OrderByDescending(review => review.ReviewedAt)
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

        var genreStats = _genres
            .Select(genre => new GenreStatViewModel
            {
                GenreName = genre.Name,
                BookCount = _bookGenres.Count(bookGenre => bookGenre.GenreId == genre.Id),
                AverageRating = CalculateGenreAverageRating(genre.Id)
            })
            .OrderByDescending(stat => stat.AverageRating)
            .ThenByDescending(stat => stat.BookCount)
            .ToList();

        return new BookDashboardViewModel
        {
            TotalBooks = _books.Count,
            TotalAuthors = _authors.Count,
            TotalUsers = _users.Count,
            OverallAverageRating = CalculateOverallAverageRating(),
            TopBooks = topBooks.Take(3).ToList(),
            RecentReviews = recentReviews,
            GenreStats = genreStats
        };
    }

    private BookCardViewModel BuildBookCard(Book book)
    {
        var relatedReviews = _reviews.Where(review => review.BookId == book.Id).ToList();
        var averageRating = relatedReviews.Count == 0 ? 0 : relatedReviews.Average(review => review.Score);

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
            ReviewCount = relatedReviews.Count
        };
    }

    private double CalculateOverallAverageRating()
    {
        return _reviews.Count == 0 ? 0 : _reviews.Average(review => review.Score);
    }

    private double CalculateGenreAverageRating(int genreId)
    {
        var genreBooks = _bookGenres
            .Where(bookGenre => bookGenre.GenreId == genreId)
            .Select(bookGenre => bookGenre.BookId)
            .Distinct()
            .ToList();

        var genreRatings = _reviews.Where(review => genreBooks.Contains(review.BookId)).ToList();

        return genreRatings.Count == 0 ? 0 : genreRatings.Average(review => review.Score);
    }

    private static (
        List<Author> authors,
        List<Publisher> publishers,
        List<Genre> genres,
        List<User> users,
        List<Book> books,
        List<BookGenre> bookGenres,
        List<Review> reviews) SeedData()
    {
        var authors = new List<Author>
        {
            new()
            {
                Id = 1,
                FirstName = "Robert",
                LastName = "Martin",
                Biography = "Poznat po knjigama o čistom kodu i softverskoj kvaliteti.",
                BirthDate = new DateTime(1952, 12, 5),
                Nationality = "American",
                Website = "https://cleancoder.com"
            },
            new()
            {
                Id = 2,
                FirstName = "Martin",
                LastName = "Fowler",
                Biography = "Autor i arhitekt softverskih sustava.",
                BirthDate = new DateTime(1963, 12, 18),
                Nationality = "British",
                Website = "https://martinfowler.com"
            },
            new()
            {
                Id = 3,
                FirstName = "Andrew",
                LastName = "Hunt",
                Biography = "Jedan od autora koji naglašava pragmatičan razvoj softvera.",
                BirthDate = new DateTime(1964, 6, 15),
                Nationality = "American",
                Website = "https://pragprog.com"
            }
        };

        var publishers = new List<Publisher>
        {
            new()
            {
                Id = 1,
                Name = "Addison-Wesley",
                City = "Boston",
                Country = "USA",
                FoundedOn = new DateTime(1942, 1, 1),
                Website = "https://www.pearson.com/en-us/subject-catalog/p/addison-wesley.html",
                ContactEmail = "contact@aw.com"
            },
            new()
            {
                Id = 2,
                Name = "Pragmatic Bookshelf",
                City = "Raleigh",
                Country = "USA",
                FoundedOn = new DateTime(2004, 1, 1),
                Website = "https://pragprog.com",
                ContactEmail = "hello@pragprog.com"
            }
        };

        var genres = new List<Genre>
        {
            new() { Id = 1, Name = "Programming", Description = "Knjige o razvoju softvera i kodiranju.", Audience = "Developers" },
            new() { Id = 2, Name = "Architecture", Description = "Teme o dizajnu i arhitekturi sustava.", Audience = "Software Architects" },
            new() { Id = 3, Name = "Productivity", Description = "Praktični savjeti za učinkovit rad.", Audience = "General" }
        };

        var users = new List<User>
        {
            new()
            {
                Id = 1,
                Username = "ana",
                FullName = "Ana Horvat",
                Email = "ana@example.com",
                JoinedAt = new DateTime(2025, 1, 12),
                FavoriteGenre = "Programming",
                ReputationPoints = 145,
                IsPremiumMember = true
            },
            new()
            {
                Id = 2,
                Username = "ivan",
                FullName = "Ivan Kovač",
                Email = "ivan@example.com",
                JoinedAt = new DateTime(2025, 3, 4),
                FavoriteGenre = "Architecture",
                ReputationPoints = 88,
                IsPremiumMember = false
            },
            new()
            {
                Id = 3,
                Username = "marta",
                FullName = "Marta Babić",
                Email = "marta@example.com",
                JoinedAt = new DateTime(2025, 5, 20),
                FavoriteGenre = "Productivity",
                ReputationPoints = 210,
                IsPremiumMember = true
            }
        };

        var books = new List<Book>
        {
            new()
            {
                Id = 1,
                Title = "Clean Code",
                Isbn = "9780132350884",
                Description = "Smjernice za pisanje čistog i održivog koda.",
                PublishedOn = new DateTime(2008, 8, 1),
                PageCount = 464,
                Language = "English",
                Status = BookStatus.Available,
                AuthorId = 1,
                PublisherId = 1
            },
            new()
            {
                Id = 2,
                Title = "Refactoring",
                Isbn = "9780134757599",
                Description = "Tehnike za poboljšanje postojeće baze koda.",
                PublishedOn = new DateTime(2018, 11, 19),
                PageCount = 448,
                Language = "English",
                Status = BookStatus.Available,
                AuthorId = 2,
                PublisherId = 1
            },
            new()
            {
                Id = 3,
                Title = "The Pragmatic Programmer",
                Isbn = "9780135957059",
                Description = "Praktičan pristup razvoju softvera.",
                PublishedOn = new DateTime(2019, 9, 13),
                PageCount = 352,
                Language = "English",
                Status = BookStatus.Reserved,
                AuthorId = 3,
                PublisherId = 2
            },
            new()
            {
                Id = 4,
                Title = "Domain-Driven Design Distilled",
                Isbn = "9780134434421",
                Description = "Uvod u DDD principe i primjenu u praksi.",
                PublishedOn = new DateTime(2016, 8, 30),
                PageCount = 176,
                Language = "English",
                Status = BookStatus.Available,
                AuthorId = 2,
                PublisherId = 2
            }
        };

        foreach (var book in books)
        {
            book.Author = authors.Single(author => author.Id == book.AuthorId);
            book.Publisher = publishers.Single(publisher => publisher.Id == book.PublisherId);
            book.Author.Books.Add(book);
            book.Publisher.Books.Add(book);
        }

        var bookGenres = new List<BookGenre>
        {
            new() { BookId = 1, GenreId = 1, AddedAt = new DateTime(2025, 2, 1) },
            new() { BookId = 1, GenreId = 2, AddedAt = new DateTime(2025, 2, 1) },
            new() { BookId = 2, GenreId = 1, AddedAt = new DateTime(2025, 2, 10) },
            new() { BookId = 2, GenreId = 2, AddedAt = new DateTime(2025, 2, 10) },
            new() { BookId = 3, GenreId = 1, AddedAt = new DateTime(2025, 2, 20) },
            new() { BookId = 3, GenreId = 3, AddedAt = new DateTime(2025, 2, 20) },
            new() { BookId = 4, GenreId = 2, AddedAt = new DateTime(2025, 3, 1) },
            new() { BookId = 4, GenreId = 3, AddedAt = new DateTime(2025, 3, 1) }
        };

        foreach (var relation in bookGenres)
        {
            relation.Book = books.Single(book => book.Id == relation.BookId);
            relation.Genre = genres.Single(genre => genre.Id == relation.GenreId);
            relation.Book.BookGenres.Add(relation);
            relation.Genre.BookGenres.Add(relation);
        }

        var reviews = new List<Review>
        {
            new()
            {
                Id = 1,
                Score = 5,
                Title = "Odlična osnova",
                Comment = "Knjiga je vrlo praktična i odmah primjenjiva.",
                ReviewedAt = new DateTime(2026, 1, 18, 10, 20, 0),
                IsRecommended = true,
                Sentiment = ReviewSentiment.Enthusiastic,
                BookId = 1,
                UserId = 1
            },
            new()
            {
                Id = 2,
                Score = 4,
                Title = "Vrlo korisno",
                Comment = "Dobar pregled principa refaktoriranja.",
                ReviewedAt = new DateTime(2026, 1, 20, 11, 30, 0),
                IsRecommended = true,
                Sentiment = ReviewSentiment.Positive,
                BookId = 2,
                UserId = 2
            },
            new()
            {
                Id = 3,
                Score = 5,
                Title = "Mora se pročitati",
                Comment = "Odlična knjiga za sve koji rade s kodom svaki dan.",
                ReviewedAt = new DateTime(2026, 1, 22, 8, 15, 0),
                IsRecommended = true,
                Sentiment = ReviewSentiment.Enthusiastic,
                BookId = 3,
                UserId = 3
            },
            new()
            {
                Id = 4,
                Score = 4,
                Title = "Dobar uvod",
                Comment = "Kratko i jasno objašnjenje DDD principa.",
                ReviewedAt = new DateTime(2026, 1, 25, 14, 45, 0),
                IsRecommended = true,
                Sentiment = ReviewSentiment.Positive,
                BookId = 4,
                UserId = 1
            },
            new()
            {
                Id = 5,
                Score = 3,
                Title = "Solidno",
                Comment = "Dobra knjiga, ali traži prethodno znanje.",
                ReviewedAt = new DateTime(2026, 1, 26, 9, 0, 0),
                IsRecommended = false,
                Sentiment = ReviewSentiment.Neutral,
                BookId = 2,
                UserId = 3
            }
        };

        foreach (var review in reviews)
        {
            review.Book = books.Single(book => book.Id == review.BookId);
            review.User = users.Single(user => user.Id == review.UserId);
            review.Book.Reviews.Add(review);
            review.User.Reviews.Add(review);
        }

        return (authors, publishers, genres, users, books, bookGenres, reviews);
    }
}