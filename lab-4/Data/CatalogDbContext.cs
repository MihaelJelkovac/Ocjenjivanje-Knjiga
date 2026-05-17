using Lab4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Lab4.Data;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<BookGenre> BookGenres { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure primary key for BookGenre (composite key)
        modelBuilder.Entity<BookGenre>()
            .HasKey(bg => new { bg.BookId, bg.GenreId });

        // Configure relationships
        modelBuilder.Entity<BookGenre>()
            .HasOne(bg => bg.Book)
            .WithMany(b => b.BookGenres)
            .HasForeignKey(bg => bg.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BookGenre>()
            .HasOne(bg => bg.Genre)
            .WithMany(g => g.BookGenres)
            .HasForeignKey(bg => bg.GenreId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Authors
        var authors = new[]
        {
            new Author { Id = 1, FirstName = "J.K.", LastName = "Rowling", Biography = "British author", BirthDate = new DateTime(1965, 7, 31), Nationality = "British", Website = "https://www.jkrowling.com" },
            new Author { Id = 2, FirstName = "George", LastName = "R. R. Martin", Biography = "American author", BirthDate = new DateTime(1948, 9, 20), Nationality = "American", Website = "https://www.georgerrmartin.com" },
            new Author { Id = 3, FirstName = "J.R.R.", LastName = "Tolkien", Biography = "English author", BirthDate = new DateTime(1892, 1, 3), Nationality = "English", Website = "https://www.tolkien.co.uk" }
        };
        modelBuilder.Entity<Author>().HasData(authors);

        // Publishers
        var publishers = new[]
        {
            new Publisher { Id = 1, Name = "Bloomsbury", City = "London", Country = "UK", FoundedOn = new DateTime(1986, 1, 1), Website = "https://www.bloomsbury.com", ContactEmail = "info@bloomsbury.com" },
            new Publisher { Id = 2, Name = "Bantam Books", City = "New York", Country = "USA", FoundedOn = new DateTime(1945, 1, 1), Website = "https://www.bantambooks.com", ContactEmail = "info@bantambooks.com" },
            new Publisher { Id = 3, Name = "Allen & Unwin", City = "London", Country = "UK", FoundedOn = new DateTime(1914, 1, 1), Website = "https://www.allenandunwin.com", ContactEmail = "info@allenandunwin.com" }
        };
        modelBuilder.Entity<Publisher>().HasData(publishers);

        // Genres
        var genres = new[]
        {
            new Genre { Id = 1, Name = "Fantasy", Description = "Fantastical worlds and magic", Audience = "Young Adults" },
            new Genre { Id = 2, Name = "Science Fiction", Description = "Future and space", Audience = "Adults" },
            new Genre { Id = 3, Name = "Drama", Description = "Human conflicts", Audience = "Adults" }
        };
        modelBuilder.Entity<Genre>().HasData(genres);

        // Books
        var books = new[]
        {
            new Book { Id = 1, Title = "Harry Potter and the Philosopher's Stone", Isbn = "978-0-7475-3269-9", Description = "A young wizard's journey", PublishedOn = new DateTime(1997, 6, 26), PageCount = 309, Language = "English", Status = BookStatus.Available, AuthorId = 1, PublisherId = 1 },
            new Book { Id = 2, Title = "A Game of Thrones", Isbn = "978-0-553-10354-1", Description = "Epic fantasy drama", PublishedOn = new DateTime(1996, 8, 6), PageCount = 694, Language = "English", Status = BookStatus.Available, AuthorId = 2, PublisherId = 2 },
            new Book { Id = 3, Title = "The Fellowship of the Ring", Isbn = "978-0-544-00362-6", Description = "Epic fantasy adventure", PublishedOn = new DateTime(1954, 7, 29), PageCount = 423, Language = "English", Status = BookStatus.Available, AuthorId = 3, PublisherId = 3 }
        };
        modelBuilder.Entity<Book>().HasData(books);

        // BookGenres (linking books to genres)
        var bookGenres = new[]
        {
            new BookGenre { BookId = 1, GenreId = 1, AddedAt = new DateTime(2024, 1, 1, 10, 0, 0) },
            new BookGenre { BookId = 2, GenreId = 1, AddedAt = new DateTime(2024, 1, 2, 10, 0, 0) },
            new BookGenre { BookId = 2, GenreId = 3, AddedAt = new DateTime(2024, 1, 2, 10, 15, 0) },
            new BookGenre { BookId = 3, GenreId = 1, AddedAt = new DateTime(2024, 1, 3, 10, 0, 0) }
        };
        modelBuilder.Entity<BookGenre>().HasData(bookGenres);

        // Users
        var users = new[]
        {
            new User { Id = 1, Username = "reader1", FullName = "Alice Reader", Email = "alice@example.com", JoinedAt = new DateTime(2023, 1, 15, 8, 30, 0), FavoriteGenre = "Fantasy", ReputationPoints = 150, IsPremiumMember = true },
            new User { Id = 2, Username = "reader2", FullName = "Bob Smith", Email = "bob@example.com", JoinedAt = new DateTime(2023, 2, 20, 9, 15, 0), FavoriteGenre = "Science Fiction", ReputationPoints = 80, IsPremiumMember = false },
            new User { Id = 3, Username = "reader3", FullName = "Carol White", Email = "carol@example.com", JoinedAt = new DateTime(2023, 3, 10, 7, 45, 0), FavoriteGenre = "Fantasy", ReputationPoints = 200, IsPremiumMember = true }
        };
        modelBuilder.Entity<User>().HasData(users);

        // Reviews
        var reviews = new[]
        {
            new Review { Id = 1, Score = 5, Title = "Amazing!", Comment = "Best book ever", ReviewedAt = new DateTime(2024, 2, 10, 14, 30, 0), IsRecommended = true, Sentiment = ReviewSentiment.Enthusiastic, BookId = 1, UserId = 1 },
            new Review { Id = 2, Score = 4, Title = "Great read", Comment = "Very engaging", ReviewedAt = new DateTime(2024, 2, 12, 16, 45, 0), IsRecommended = true, Sentiment = ReviewSentiment.Positive, BookId = 2, UserId = 2 },
            new Review { Id = 3, Score = 5, Title = "Epic!", Comment = "A masterpiece", ReviewedAt = new DateTime(2024, 2, 15, 11, 20, 0), IsRecommended = true, Sentiment = ReviewSentiment.Enthusiastic, BookId = 3, UserId = 3 }
        };
        modelBuilder.Entity<Review>().HasData(reviews);
    }
}
