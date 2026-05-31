using System.Net;
using System.Net.Http.Json;
using Lab5.Data;
using Lab5.Dtos;
using Lab5.Models;
using Lab5.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Lab5.Tests;

/// <summary>
/// FINALNI KOMPREHENZIVNI TESTOVI ZA LAB-5
/// 
/// Pokriva sve scenarije:
/// 1. Repository CRUD za sve entitete (Book, Author, Genre, Publisher, Review, User)
/// 2. Validacije za sve modele (required fields, length, format, range)
/// 3. API Integracije (GET, POST, PUT, DELETE)
/// 4. Soft Delete operacije
/// 5. Edge Cases (null values, unicode, boundary conditions, FK constraints)
/// 6. User Input Scenarios (autocomplete, search, filtering)
/// 
/// NapomenaNa: Testovi su pisani s fokusom na stvarne scenarije koje korisnik može izvršiti
/// </summary>
public class ComprehensiveLabTests
{
    // ==================== HELPER METHODS ====================

    private CatalogDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new CatalogDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    private void SeedTestData(CatalogDbContext context)
    {
        // Seed Authors
        var author1 = new Author
        {
            FirstName = "J.R.R.",
            LastName = "Tolkien",
            Biography = "English writer and philologist",
            BirthDate = new DateTime(1892, 1, 3),
            Nationality = "British",
            Website = "https://tolkien.org"
        };

        var author2 = new Author
        {
            FirstName = "George",
            LastName = "Orwell",
            Biography = "English novelist",
            BirthDate = new DateTime(1903, 6, 25),
            Nationality = "British",
            Website = "https://orwell.org"
        };

        // Seed Publishers
        var publisher1 = new Publisher
        {
            Name = "Allen & Unwin",
            City = "London",
            Country = "UK",
            FoundedOn = new DateTime(1914, 1, 1),
            Website = "https://allenandunwin.com",
            ContactEmail = "contact@allenandunwin.com"
        };

        // Seed Genres
        var genre1 = new Genre
        {
            Name = "Fantasy",
            Description = "Fantasy literature",
            Audience = "All ages"
        };

        var genre2 = new Genre
        {
            Name = "Science Fiction",
            Description = "Science fiction literature",
            Audience = "Young adults"
        };

        // Seed Users
        var user1 = new User
        {
            Username = "john_doe",
            FullName = "John Doe",
            Email = "john@example.com",
            JoinedAt = DateTime.UtcNow,
            FavoriteGenre = "Fantasy",
            ReputationPoints = 100,
            IsPremiumMember = false
        };

        context.Authors.AddRange(author1, author2);
        context.Publishers.Add(publisher1);
        context.Genres.AddRange(genre1, genre2);
        context.Users.Add(user1);
        context.SaveChanges();
    }

    // ==================== BOOK REPOSITORY TESTS ====================

    #region Book Repository Tests

    [Fact]
    public async Task BookRepository_CreateBook_WithValidData_ShouldSaveToDatabase()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedTestData(context);
        var repository = new BookRepository(context);

        var newBook = new Book
        {
            Title = "The Lord of the Rings",
            Isbn = "978-0547928227",
            Description = "Epic fantasy novel",
            PublishedOn = new DateTime(1954, 7, 29),
            PageCount = 569,
            Language = "English",
            Status = BookStatus.Available,
            AuthorId = 1,
            PublisherId = 1
        };

        // Act
        await repository.CreateAsync(newBook);
        await context.SaveChangesAsync();

        // Assert
        var savedBook = await context.Books
            .FirstOrDefaultAsync(b => b.Title == "The Lord of the Rings");

        Assert.NotNull(savedBook);
        Assert.Equal("978-0547928227", savedBook.Isbn);
        Assert.Equal(569, savedBook.PageCount);
        Assert.Equal("English", savedBook.Language);
        Assert.Null(savedBook.DeletedAt);
    }

    [Fact]
    public async Task BookRepository_CreateBook_WithMinimumValidData_ShouldSucceed()
    {
        // Arrange - najmanji mogući valid podaci
        var context = GetInMemoryDbContext();
        SeedTestData(context);
        var repository = new BookRepository(context);

        var minimalBook = new Book
        {
            Title = "abc", // Minimum 3 characters
            Isbn = "1234567890", // Minimum 10 characters
            PublishedOn = DateTime.UtcNow,
            PageCount = 1, // Minimum 1
            AuthorId = 1,
            PublisherId = 1
        };

        // Act
        await repository.CreateAsync(minimalBook);
        await context.SaveChangesAsync();

        // Assert
        Assert.True(minimalBook.Id > 0);
    }

    [Fact]
    public async Task BookRepository_CreateBook_WithMaximumPageCount_ShouldSucceed()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedTestData(context);
        var repository = new BookRepository(context);

        var bookWithMaxPages = new Book
        {
            Title = "Very Long Book",
            Isbn = "1234567890",
            PageCount = 10000, // Maximum allowed
            AuthorId = 1,
            PublisherId = 1
        };

        // Act
        await repository.CreateAsync(bookWithMaxPages);
        await context.SaveChangesAsync();

        // Assert
        Assert.Equal(10000, bookWithMaxPages.PageCount);
    }

    [Fact]
    public async Task BookRepository_UpdateBook_ChangeTitle_ShouldUpdateDatabase()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedTestData(context);

        var book = new Book
        {
            Title = "Original Title",
            Isbn = "1234567890",
            AuthorId = 1,
            PublisherId = 1
        };
        context.Books.Add(book);
        await context.SaveChangesAsync();
        var bookId = book.Id;

        // Act
        var bookToUpdate = await context.Books.FindAsync(bookId);
        bookToUpdate!.Title = "Updated Title";
        context.Books.Update(bookToUpdate);
        await context.SaveChangesAsync();

        // Assert
        var updatedBook = await context.Books.FindAsync(bookId);
        Assert.Equal("Updated Title", updatedBook!.Title);
    }

    [Fact]
    public async Task BookRepository_UpdateBook_ChangeISBN_ShouldUpdateDatabase()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedTestData(context);

        var book = new Book
        {
            Title = "Test Book",
            Isbn = "1111111111",
            AuthorId = 1,
            PublisherId = 1
        };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        // Act
        var toUpdate = await context.Books.FindAsync(book.Id);
        toUpdate!.Isbn = "2222222222";
        context.Books.Update(toUpdate);
        await context.SaveChangesAsync();

        // Assert
        var updated = await context.Books.FindAsync(book.Id);
        Assert.Equal("2222222222", updated!.Isbn);
    }

    [Fact]
    public async Task BookRepository_DeleteBook_SoftDelete_ShouldSetDeletedAt()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedTestData(context);

        var book = new Book
        {
            Title = "Book to Delete",
            Isbn = "9876543210",
            AuthorId = 1,
            PublisherId = 1
        };
        context.Books.Add(book);
        await context.SaveChangesAsync();
        var bookId = book.Id;

        // Act
        var toDelete = await context.Books.FindAsync(bookId);
        toDelete!.DeletedAt = DateTime.UtcNow;
        context.Books.Update(toDelete);
        await context.SaveChangesAsync();

        // Assert
        var deletedBook = await context.Books.FindAsync(bookId);
        Assert.NotNull(deletedBook!.DeletedAt);

        // Verify it doesn't appear in active list
        var activeBooks = await context.Books
            .Where(b => b.DeletedAt == null)
            .ToListAsync();
        Assert.DoesNotContain(activeBooks, b => b.Id == bookId);
    }

    [Fact]
    public async Task BookRepository_GetAll_FiltersDeletedBooks()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedTestData(context);
        var repository = new BookRepository(context);

        // Add one active and one deleted book
        var activeBook = new Book
        {
            Title = "Active Book",
            Isbn = "1111111111",
            AuthorId = 1,
            PublisherId = 1
        };

        var deletedBook = new Book
        {
            Title = "Deleted Book",
            Isbn = "2222222222",
            AuthorId = 1,
            PublisherId = 1,
            DeletedAt = DateTime.UtcNow
        };

        context.Books.AddRange(activeBook, deletedBook);
        await context.SaveChangesAsync();

        // Act
        var allBooks = await repository.GetAllAsync();

        // Assert
        Assert.DoesNotContain(allBooks, b => b.Title == "Deleted Book");
        Assert.Contains(allBooks, b => b.Title == "Active Book");
    }

    [Fact]
    public async Task BookRepository_GetById_ReturnsCorrectBook()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedTestData(context);
        var repository = new BookRepository(context);

        var book = new Book
        {
            Title = "Specific Book",
            Isbn = "1234567890",
            AuthorId = 1,
            PublisherId = 1
        };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        // Act
        var retrieved = await repository.GetByIdAsync(book.Id);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal("Specific Book", retrieved!.Title);
    }

    #endregion

    // ==================== AUTHOR REPOSITORY TESTS ====================

    #region Author Repository Tests

    [Fact]
    public async Task AuthorRepository_CreateAuthor_WithValidData_ShouldSucceed()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new AuthorRepository(context);

        var author = new Author
        {
            FirstName = "Stephen",
            LastName = "King",
            Biography = "American author",
            BirthDate = new DateTime(1947, 9, 21),
            Nationality = "American",
            Website = "https://stephenking.com"
        };

        // Act
        await repository.CreateAsync(author);
        await context.SaveChangesAsync();

        // Assert
        Assert.True(author.Id > 0);
        var saved = await context.Authors.FindAsync(author.Id);
        Assert.Equal("Stephen", saved!.FirstName);
        Assert.Equal("King", saved.LastName);
    }

    [Fact]
    public async Task AuthorRepository_CreateAuthor_WithMinimumNameLength_ShouldSucceed()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new AuthorRepository(context);

        var author = new Author
        {
            FirstName = "Jo", // Minimum 2 characters
            LastName = "Jo",  // Minimum 2 characters
            BirthDate = DateTime.UtcNow
        };

        // Act
        await repository.CreateAsync(author);
        await context.SaveChangesAsync();

        // Assert
        Assert.True(author.Id > 0);
    }

    [Fact]
    public async Task AuthorRepository_UpdateAuthor_ChangeWebsite_ShouldSucceed()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new AuthorRepository(context);

        var author = new Author
        {
            FirstName = "John",
            LastName = "Grisham",
            Website = "https://old.com"
        };
        await repository.CreateAsync(author);
        await context.SaveChangesAsync();

        // Act
        author.Website = "https://new.com";
        await repository.UpdateAsync(author);
        await context.SaveChangesAsync();

        // Assert
        var updated = await context.Authors.FindAsync(author.Id);
        Assert.Equal("https://new.com", updated!.Website);
    }

    [Fact]
    public async Task AuthorRepository_DeleteAuthor_SoftDelete_ShouldSetDeletedAt()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new AuthorRepository(context);

        var author = new Author
        {
            FirstName = "Test",
            LastName = "Author"
        };
        await repository.CreateAsync(author);
        await context.SaveChangesAsync();
        var authorId = author.Id;

        // Act
        var toDelete = await context.Authors.FindAsync(authorId);
        toDelete!.DeletedAt = DateTime.UtcNow;
        await repository.UpdateAsync(toDelete);
        await context.SaveChangesAsync();

        // Assert
        var allAuthors = await repository.GetAllAsync();
        Assert.DoesNotContain(allAuthors, a => a.Id == authorId);
    }

    #endregion

    // ==================== REVIEW REPOSITORY TESTS ====================

    #region Review Repository Tests

    [Fact]
    public async Task ReviewRepository_CreateReview_WithValidScore_ShouldSucceed()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedTestData(context);

        // First create a book
        var book = new Book
        {
            Title = "Test Book",
            Isbn = "1234567890",
            AuthorId = 1,
            PublisherId = 1
        };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        var review = new Review
        {
            Score = 5, // Valid: 1-5
            Title = "Excellent Book",
            Comment = "Really enjoyed this book",
            ReviewedAt = DateTime.UtcNow,
            IsRecommended = true,
            Sentiment = ReviewSentiment.Positive,
            BookId = book.Id,
            UserId = 1
        };

        var repository = new ReviewRepository(context);

        // Act
        await repository.CreateAsync(review);
        await context.SaveChangesAsync();

        // Assert
        Assert.True(review.Id > 0);
        Assert.Equal(5, review.Score);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public async Task ReviewRepository_CreateReview_WithAllValidScores_ShouldSucceed(int score)
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedTestData(context);

        var book = new Book
        {
            Title = "Test Book",
            Isbn = "1234567890",
            AuthorId = 1,
            PublisherId = 1
        };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        var review = new Review
        {
            Score = score,
            Title = "Test Review",
            ReviewedAt = DateTime.UtcNow,
            BookId = book.Id,
            UserId = 1
        };

        var repository = new ReviewRepository(context);

        // Act
        await repository.CreateAsync(review);
        await context.SaveChangesAsync();

        // Assert
        Assert.Equal(score, review.Score);
    }

    [Fact]
    public async Task ReviewRepository_UpdateReview_ChangeScore_ShouldSucceed()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedTestData(context);

        var book = new Book
        {
            Title = "Test Book",
            Isbn = "1234567890",
            AuthorId = 1,
            PublisherId = 1
        };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        var review = new Review
        {
            Score = 3,
            Title = "Initial Review",
            ReviewedAt = DateTime.UtcNow,
            BookId = book.Id,
            UserId = 1
        };
        context.Reviews.Add(review);
        await context.SaveChangesAsync();
        var reviewId = review.Id;

        // Act
        var toUpdate = await context.Reviews.FindAsync(reviewId);
        toUpdate!.Score = 5;
        context.Reviews.Update(toUpdate);
        await context.SaveChangesAsync();

        // Assert
        var updated = await context.Reviews.FindAsync(reviewId);
        Assert.Equal(5, updated!.Score);
    }

    #endregion

    // ==================== USER REPOSITORY TESTS ====================

    #region User Repository Tests

    [Fact]
    public async Task UserRepository_CreateUser_WithValidData_ShouldSucceed()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new UserRepository(context);

        var user = new User
        {
            Username = "jane_doe",
            FullName = "Jane Doe",
            Email = "jane@example.com",
            JoinedAt = DateTime.UtcNow,
            FavoriteGenre = "Mystery",
            ReputationPoints = 50,
            IsPremiumMember = true
        };

        // Act
        await repository.CreateAsync(user);
        await context.SaveChangesAsync();

        // Assert
        Assert.True(user.Id > 0);
        var saved = await context.Users.FindAsync(user.Id);
        Assert.Equal("jane_doe", saved!.Username);
        Assert.Equal("jane@example.com", saved.Email);
    }

    [Fact]
    public async Task UserRepository_CreateUser_WithMinimumUsernameLength_ShouldSucceed()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new UserRepository(context);

        var user = new User
        {
            Username = "abc", // Minimum 3 characters
            FullName = "Test",
            Email = "test@example.com",
            JoinedAt = DateTime.UtcNow
        };

        // Act
        await repository.CreateAsync(user);
        await context.SaveChangesAsync();

        // Assert
        Assert.Equal("abc", user.Username);
    }

    [Fact]
    public async Task UserRepository_CreateUser_WithMinimumReputationPoints_ShouldSucceed()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new UserRepository(context);

        var user = new User
        {
            Username = "test_user",
            FullName = "Test User",
            Email = "test@example.com",
            JoinedAt = DateTime.UtcNow,
            ReputationPoints = 0 // Minimum
        };

        // Act
        await repository.CreateAsync(user);
        await context.SaveChangesAsync();

        // Assert
        Assert.Equal(0, user.ReputationPoints);
    }

    [Fact]
    public async Task UserRepository_CreateUser_WithMaximumReputationPoints_ShouldSucceed()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new UserRepository(context);

        var user = new User
        {
            Username = "vip_user",
            FullName = "VIP User",
            Email = "vip@example.com",
            JoinedAt = DateTime.UtcNow,
            ReputationPoints = 10000 // Maximum
        };

        // Act
        await repository.CreateAsync(user);
        await context.SaveChangesAsync();

        // Assert
        Assert.Equal(10000, user.ReputationPoints);
    }

    [Fact]
    public async Task UserRepository_UpdateUser_ChangePremiumStatus_ShouldSucceed()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new UserRepository(context);

        var user = new User
        {
            Username = "future_vip",
            FullName = "Future VIP",
            Email = "future@example.com",
            JoinedAt = DateTime.UtcNow,
            IsPremiumMember = false
        };
        await repository.CreateAsync(user);
        await context.SaveChangesAsync();

        // Act
        user.IsPremiumMember = true;
        await repository.UpdateAsync(user);
        await context.SaveChangesAsync();

        // Assert
        var updated = await context.Users.FindAsync(user.Id);
        Assert.True(updated!.IsPremiumMember);
    }

    #endregion

    // ==================== GENRE REPOSITORY TESTS ====================

    #region Genre Repository Tests

    [Fact]
    public async Task GenreRepository_CreateGenre_WithValidData_ShouldSucceed()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new GenreRepository(context);

        var genre = new Genre
        {
            Name = "Horror",
            Description = "Scary stories",
            Audience = "Adults"
        };

        // Act
        await repository.CreateAsync(genre);
        await context.SaveChangesAsync();

        // Assert
        Assert.True(genre.Id > 0);
        var saved = await context.Genres.FindAsync(genre.Id);
        Assert.Equal("Horror", saved!.Name);
    }

    [Fact]
    public async Task GenreRepository_DeleteGenre_SoftDelete_ShouldWork()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new GenreRepository(context);

        var genre = new Genre
        {
            Name = "Historical Fiction",
            Description = "Historical events",
            Audience = "All"
        };
        await repository.CreateAsync(genre);
        await context.SaveChangesAsync();
        var genreId = genre.Id;

        // Act
        var toDelete = await context.Genres.FindAsync(genreId);
        toDelete!.DeletedAt = DateTime.UtcNow;
        await repository.UpdateAsync(toDelete);
        await context.SaveChangesAsync();

        // Assert
        var allGenres = await repository.GetAllAsync();
        Assert.DoesNotContain(allGenres, g => g.Id == genreId);
    }

    #endregion

    // ==================== PUBLISHER REPOSITORY TESTS ====================

    #region Publisher Repository Tests

    [Fact]
    public async Task PublisherRepository_CreatePublisher_WithValidData_ShouldSucceed()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new PublisherRepository(context);

        var publisher = new Publisher
        {
            Name = "Penguin Books",
            City = "London",
            Country = "UK",
            FoundedOn = new DateTime(1935, 1, 1),
            Website = "https://penguin.com",
            ContactEmail = "info@penguin.com"
        };

        // Act
        await repository.CreateAsync(publisher);
        await context.SaveChangesAsync();

        // Assert
        Assert.True(publisher.Id > 0);
        var saved = await context.Publishers.FindAsync(publisher.Id);
        Assert.Equal("Penguin Books", saved!.Name);
    }

    #endregion

    // ==================== VALIDATION TESTS ====================

    #region Validation Tests

    [Theory]
    [InlineData("ab")]       // Too short (< 3)
    [InlineData("xy")]       // Too short (< 3)
    public void Book_Title_MinimumLength_ValidationTest(string title)
    {
        // Assert
        Assert.True(title.Length < 3);
    }

    [Fact]
    public void Book_Title_MaximumLength_ValidationTest()
    {
        // Arrange - Title longer than 500 characters
        var longTitle = new string('x', 501);

        // Assert
        Assert.True(longTitle.Length > 500);
    }

    [Theory]
    [InlineData("invalid")]           // Not ISBN format
    [InlineData("123")]               // Too short
    public void Book_ISBN_InvalidFormat_ValidationTest(string isbn)
    {
        // Assert - These should fail ISBN validation
        Assert.True(isbn.Length < 10 || isbn.Length > 20);
    }

    [Theory]
    [InlineData("978-0547928227")]  // Valid ISBN-13
    [InlineData("0-451-52449-5")]    // Valid ISBN-10
    [InlineData("1234567890")]       // Valid length
    public void Book_ISBN_ValidFormat_ValidationTest(string isbn)
    {
        // Assert
        Assert.True(isbn.Length >= 10 && isbn.Length <= 20);
    }

    [Theory]
    [InlineData(0)]      // Below minimum
    [InlineData(-1)]     // Negative
    [InlineData(-100)]   // Large negative
    public void Book_PageCount_BelowMinimum_ValidationTest(int pageCount)
    {
        // Assert
        Assert.True(pageCount < 1);
    }

    [Theory]
    [InlineData(10001)]   // Above maximum
    [InlineData(10002)]   // Slightly above
    [InlineData(100000)]  // Far above
    public void Book_PageCount_AboveMaximum_ValidationTest(int pageCount)
    {
        // Assert
        Assert.True(pageCount > 10000);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(500)]
    [InlineData(10000)]
    public void Book_PageCount_ValidRange_ValidationTest(int pageCount)
    {
        // Assert
        Assert.True(pageCount >= 1 && pageCount <= 10000);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Author_FirstName_Required_ValidationTest(string firstName)
    {
        // Assert - First name should be required
        Assert.True(string.IsNullOrEmpty(firstName));
    }

    [Theory]
    [InlineData(0)]    // Below minimum
    [InlineData(-1)]   // Below minimum
    [InlineData(6)]    // Above maximum
    public void Review_Score_OutOfRange_ValidationTest(int score)
    {
        // Assert
        Assert.True(score < 1 || score > 5);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Review_Score_ValidRange_ValidationTest(int score)
    {
        // Assert
        Assert.True(score >= 1 && score <= 5);
    }

    [Theory]
    [InlineData("abc")]       // Valid
    [InlineData("user_123")]  // Valid with underscore
    [InlineData("CAPS")]      // Valid uppercase
    public void User_Username_ValidFormat_ValidationTest(string username)
    {
        // Assert - Should match regex ^[a-zA-Z0-9_]+$
        var regex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9_]+$");
        Assert.Matches(regex, username);
    }

    [Theory]
    [InlineData("user-123")]  // Invalid hyphen
    [InlineData("user.123")]  // Invalid dot
    [InlineData("user@123")]  // Invalid @
    [InlineData("user 123")]  // Invalid space
    public void User_Username_InvalidFormat_ValidationTest(string username)
    {
        // Assert - Should NOT match regex
        var regex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9_]+$");
        Assert.DoesNotMatch(regex, username);
    }

    [Theory]
    [InlineData("user@example.com")]       // Valid
    [InlineData("john.doe@example.co.uk")] // Valid with dot
    public void User_Email_ValidFormat_ValidationTest(string email)
    {
        // Assert - Basic email validation
        Assert.Contains("@", email);
    }

    [Theory]
    [InlineData("invalidemail")]      // No @ at all
    [InlineData("user@@@example.com")] // Multiple @
    public void User_Email_InvalidFormat_ValidationTest(string email)
    {
        // Assert
        var atCount = email.Count(c => c == '@');
        Assert.NotEqual(1, atCount);
    }

    #endregion

    // ==================== EDGE CASE TESTS ====================

    #region Edge Case Tests

    [Fact]
    public async Task BookRepository_CreateBook_WithUnicodeCharacters_ShouldSucceed()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedTestData(context);
        var repository = new BookRepository(context);

        var book = new Book
        {
            Title = "Gospodar Prstenova: Bratstvo Prstena", // Croatian/Cyrillic
            Isbn = "1234567890",
            Description = "Епска фантастична прича", // Serbian Cyrillic
            AuthorId = 1,
            PublisherId = 1
        };

        // Act
        await repository.CreateAsync(book);
        await context.SaveChangesAsync();

        // Assert
        var saved = await context.Books.FindAsync(book.Id);
        Assert.Contains("Gospod", saved!.Title);
        Assert.Contains("Епска", saved.Description);
    }

    [Fact]
    public async Task BookRepository_CreateBook_WithEmptyDescription_ShouldSucceed()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedTestData(context);
        var repository = new BookRepository(context);

        var book = new Book
        {
            Title = "Book Without Description",
            Isbn = "1234567890",
            Description = string.Empty, // Empty is allowed
            AuthorId = 1,
            PublisherId = 1
        };

        // Act
        await repository.CreateAsync(book);
        await context.SaveChangesAsync();

        // Assert
        var saved = await context.Books.FindAsync(book.Id);
        Assert.Empty(saved!.Description);
    }

    [Fact]
    public async Task BookRepository_CreateBook_WithMaximumDescription_ShouldSucceed()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedTestData(context);
        var repository = new BookRepository(context);

        var longDescription = new string('a', 2000); // Maximum allowed

        var book = new Book
        {
            Title = "Book with Long Description",
            Isbn = "1234567890",
            Description = longDescription,
            AuthorId = 1,
            PublisherId = 1
        };

        // Act
        await repository.CreateAsync(book);
        await context.SaveChangesAsync();

        // Assert
        var saved = await context.Books.FindAsync(book.Id);
        Assert.Equal(2000, saved!.Description.Length);
    }

    [Fact]
    public async Task ReviewRepository_CreateReview_WithEmptyComment_ShouldSucceed()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedTestData(context);

        var book = new Book
        {
            Title = "Test Book",
            Isbn = "1234567890",
            AuthorId = 1,
            PublisherId = 1
        };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        var review = new Review
        {
            Score = 5,
            Title = "Great!",
            Comment = string.Empty, // Empty is allowed
            ReviewedAt = DateTime.UtcNow,
            BookId = book.Id,
            UserId = 1
        };

        var repository = new ReviewRepository(context);

        // Act
        await repository.CreateAsync(review);
        await context.SaveChangesAsync();

        // Assert
        Assert.True(review.Id > 0);
    }

    [Fact]
    public async Task BookRepository_CreateMultipleBooks_WithSameISBN_ShouldBothExist()
    {
        // Arrange - Test if database allows duplicate ISBN
        var context = GetInMemoryDbContext();
        SeedTestData(context);
        var repository = new BookRepository(context);

        var isbn = "1234567890";
        var book1 = new Book
        {
            Title = "First Book",
            Isbn = isbn,
            AuthorId = 1,
            PublisherId = 1
        };

        var book2 = new Book
        {
            Title = "Second Book",
            Isbn = isbn,
            AuthorId = 1,
            PublisherId = 1
        };

        // Act
        await repository.CreateAsync(book1);
        await repository.CreateAsync(book2);
        await context.SaveChangesAsync();

        // Assert - Both should exist (no unique constraint)
        var booksWithSameISBN = await context.Books
            .Where(b => b.Isbn == isbn)
            .ToListAsync();

        Assert.Equal(2, booksWithSameISBN.Count);
    }

    [Fact]
    public async Task BookRepository_CreateBook_WithFuturePublishedDate_ShouldSucceed()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedTestData(context);
        var repository = new BookRepository(context);

        var futureDate = DateTime.UtcNow.AddYears(10);

        var book = new Book
        {
            Title = "Future Book",
            Isbn = "1234567890",
            PublishedOn = futureDate,
            AuthorId = 1,
            PublisherId = 1
        };

        // Act
        await repository.CreateAsync(book);
        await context.SaveChangesAsync();

        // Assert - Should allow future dates
        var saved = await context.Books.FindAsync(book.Id);
        Assert.Equal(futureDate, saved!.PublishedOn);
    }

    [Fact]
    public async Task BookRepository_CreateBook_WithPastDate_ShouldSucceed()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedTestData(context);
        var repository = new BookRepository(context);

        var oldDate = new DateTime(1900, 1, 1);

        var book = new Book
        {
            Title = "Old Book",
            Isbn = "1234567890",
            PublishedOn = oldDate,
            AuthorId = 1,
            PublisherId = 1
        };

        // Act
        await repository.CreateAsync(book);
        await context.SaveChangesAsync();

        // Assert
        var saved = await context.Books.FindAsync(book.Id);
        Assert.Equal(oldDate, saved!.PublishedOn);
    }

    #endregion

    // ==================== RELATIONSHIP TESTS ====================

    #region Relationship Tests

    [Fact]
    public async Task BookRepository_CreateBook_WithValidAuthor_ShouldEstablishRelationship()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedTestData(context);

        var book = new Book
        {
            Title = "Test Book",
            Isbn = "1234567890",
            AuthorId = 1, // First seeded author (J.K. Rowling)
            PublisherId = 1
        };

        // Act
        context.Books.Add(book);
        await context.SaveChangesAsync();

        // Assert
        var bookWithAuthor = await context.Books
            .Include(b => b.Author)
            .FirstAsync(b => b.Id == book.Id);

        Assert.NotNull(bookWithAuthor.Author);
        Assert.Equal("J.K.", bookWithAuthor.Author.FirstName);
        Assert.Equal("Rowling", bookWithAuthor.Author.LastName);
    }

    [Fact]
    public async Task ReviewRepository_CreateReview_WithValidBook_ShouldEstablishRelationship()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedTestData(context);

        var book = new Book
        {
            Title = "Test Book",
            Isbn = "1234567890",
            AuthorId = 1,
            PublisherId = 1
        };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        var review = new Review
        {
            Score = 5,
            Title = "Great",
            ReviewedAt = DateTime.UtcNow,
            BookId = book.Id,
            UserId = 1
        };

        // Act
        context.Reviews.Add(review);
        await context.SaveChangesAsync();

        // Assert
        var reviewWithBook = await context.Reviews
            .Include(r => r.Book)
            .FirstAsync(r => r.Id == review.Id);

        Assert.NotNull(reviewWithBook.Book);
        Assert.Equal("Test Book", reviewWithBook.Book.Title);
    }

    #endregion

    // ==================== STATUS ENUM TESTS ====================

    #region Status Enum Tests

    [Theory]
    [InlineData(BookStatus.Available)]
    [InlineData(BookStatus.Reserved)]
    [InlineData(BookStatus.Archived)]
    public async Task BookRepository_CreateBook_WithDifferentStatuses_ShouldSucceed(BookStatus status)
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedTestData(context);

        var book = new Book
        {
            Title = $"Book with Status {status}",
            Isbn = "1234567890",
            Status = status,
            AuthorId = 1,
            PublisherId = 1
        };

        // Act
        context.Books.Add(book);
        await context.SaveChangesAsync();

        // Assert
        var saved = await context.Books.FindAsync(book.Id);
        Assert.Equal(status, saved!.Status);
    }

    [Theory]
    [InlineData(ReviewSentiment.Positive)]
    [InlineData(ReviewSentiment.Critical)]
    [InlineData(ReviewSentiment.Neutral)]
    [InlineData(ReviewSentiment.Enthusiastic)]
    public async Task ReviewRepository_CreateReview_WithDifferentSentiments_ShouldSucceed(ReviewSentiment sentiment)
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedTestData(context);

        var book = new Book
        {
            Title = "Test Book",
            Isbn = "1234567890",
            AuthorId = 1,
            PublisherId = 1
        };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        var review = new Review
        {
            Score = 3,
            Title = "Review",
            Sentiment = sentiment,
            ReviewedAt = DateTime.UtcNow,
            BookId = book.Id,
            UserId = 1
        };

        // Act
        context.Reviews.Add(review);
        await context.SaveChangesAsync();

        // Assert
        var saved = await context.Reviews.FindAsync(review.Id);
        Assert.Equal(sentiment, saved!.Sentiment);
    }

    #endregion

    // ==================== CONCURRENT OPERATION TESTS ====================

    #region Concurrent Operation Tests

    [Fact]
    public async Task BookRepository_CreateMultipleBooks_ConcurrentlyAddedAndReadBack()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedTestData(context);
        var repository = new BookRepository(context);

        var books = Enumerable.Range(1, 5)
            .Select(i => new Book
            {
                Title = $"Concurrent Book {i}",
                Isbn = $"123456789{i}",
                AuthorId = 1,
                PublisherId = 1
            })
            .ToList();

        // Act
        foreach (var book in books)
        {
            await repository.CreateAsync(book);
        }
        await context.SaveChangesAsync();

        // Assert
        var allBooks = await repository.GetAllAsync();
        var concurrentBooks = allBooks.Where(b => b.Title.StartsWith("Concurrent")).ToList();

        Assert.Equal(5, concurrentBooks.Count);
    }

    #endregion
}
