using System.ComponentModel.DataAnnotations;
using Lab5.Models;

namespace Lab5.Dtos;

public sealed class AuthorDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Biography { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Nationality { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
}

public sealed class AuthorUpsertDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string LastName { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Biography { get; set; } = string.Empty;

    public DateTime BirthDate { get; set; }

    [StringLength(100)]
    public string Nationality { get; set; } = string.Empty;

    [StringLength(500)]
    [Url]
    public string Website { get; set; } = string.Empty;
}

public sealed class GenreDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}

public sealed class GenreUpsertDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [StringLength(100)]
    public string Audience { get; set; } = string.Empty;
}

public sealed class PublisherDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public DateTime FoundedOn { get; set; }
    public string Website { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
}

public sealed class PublisherUpsertDto
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string City { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Country { get; set; } = string.Empty;

    public DateTime FoundedOn { get; set; }

    [StringLength(500)]
    [Url]
    public string Website { get; set; } = string.Empty;

    [StringLength(150)]
    [EmailAddress]
    public string ContactEmail { get; set; } = string.Empty;
}

public sealed class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
    public string FavoriteGenre { get; set; } = string.Empty;
    public int ReputationPoints { get; set; }
    public bool IsPremiumMember { get; set; }
}

public sealed class UserUpsertDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    public DateTime JoinedAt { get; set; }

    [StringLength(100)]
    public string FavoriteGenre { get; set; } = string.Empty;

    [Range(0, 10000)]
    public int ReputationPoints { get; set; }

    public bool IsPremiumMember { get; set; }
}

public sealed class BookSummaryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
}

public sealed class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime PublishedOn { get; set; }
    public int PageCount { get; set; }
    public string Language { get; set; } = string.Empty;
    public BookStatus Status { get; set; }
    public AuthorDto? Author { get; set; }
    public PublisherDto? Publisher { get; set; }
    public List<GenreDto> Genres { get; set; } = new();
}

public sealed class BookUpsertDto
{
    [Required]
    [StringLength(500, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(20, MinimumLength = 10)]
    public string Isbn { get; set; } = string.Empty;

    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    public DateTime PublishedOn { get; set; }

    [Range(1, 10000)]
    public int PageCount { get; set; }

    [StringLength(50)]
    public string Language { get; set; } = string.Empty;

    public BookStatus Status { get; set; }

    [Range(1, int.MaxValue)]
    public int AuthorId { get; set; }

    [Range(1, int.MaxValue)]
    public int PublisherId { get; set; }
}

public sealed class ReviewDto
{
    public int Id { get; set; }
    public int Score { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public DateTime ReviewedAt { get; set; }
    public bool IsRecommended { get; set; }
    public ReviewSentiment Sentiment { get; set; }
    public BookSummaryDto? Book { get; set; }
    public UserDto? User { get; set; }
}

public sealed class ReviewUpsertDto
{
    [Range(1, 5)]
    public int Score { get; set; }

    [Required]
    [StringLength(300, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string Comment { get; set; } = string.Empty;

    public DateTime ReviewedAt { get; set; }

    public bool IsRecommended { get; set; }

    public ReviewSentiment Sentiment { get; set; }

    [Range(1, int.MaxValue)]
    public int BookId { get; set; }

    [Range(1, int.MaxValue)]
    public int UserId { get; set; }
}

public static class ApiDtoMapper
{
    public static AuthorDto ToDto(Author author) => new()
    {
        Id = author.Id,
        FirstName = author.FirstName,
        LastName = author.LastName,
        Biography = author.Biography,
        BirthDate = author.BirthDate,
        Nationality = author.Nationality,
        Website = author.Website
    };

    public static GenreDto ToDto(Genre genre) => new()
    {
        Id = genre.Id,
        Name = genre.Name,
        Description = genre.Description,
        Audience = genre.Audience
    };

    public static PublisherDto ToDto(Publisher publisher) => new()
    {
        Id = publisher.Id,
        Name = publisher.Name,
        City = publisher.City,
        Country = publisher.Country,
        FoundedOn = publisher.FoundedOn,
        Website = publisher.Website,
        ContactEmail = publisher.ContactEmail
    };

    public static UserDto ToDto(User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        FullName = user.FullName,
        Email = user.Email,
        JoinedAt = user.JoinedAt,
        FavoriteGenre = user.FavoriteGenre,
        ReputationPoints = user.ReputationPoints,
        IsPremiumMember = user.IsPremiumMember
    };

    public static BookSummaryDto ToSummaryDto(Book book) => new()
    {
        Id = book.Id,
        Title = book.Title
    };

    public static BookDto ToDto(Book book) => new()
    {
        Id = book.Id,
        Title = book.Title,
        Isbn = book.Isbn,
        Description = book.Description,
        PublishedOn = book.PublishedOn,
        PageCount = book.PageCount,
        Language = book.Language,
        Status = book.Status,
        Author = book.Author is null ? null : ToDto(book.Author),
        Publisher = book.Publisher is null ? null : ToDto(book.Publisher),
        Genres = book.BookGenres.Select(bg => ToDto(bg.Genre)).ToList()
    };

    public static ReviewDto ToDto(Review review) => new()
    {
        Id = review.Id,
        Score = review.Score,
        Title = review.Title,
        Comment = review.Comment,
        ReviewedAt = review.ReviewedAt,
        IsRecommended = review.IsRecommended,
        Sentiment = review.Sentiment,
        Book = review.Book is null ? null : ToSummaryDto(review.Book),
        User = review.User is null ? null : ToDto(review.User)
    };
}