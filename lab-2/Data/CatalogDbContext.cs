using Lab2.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab2.Data;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
        : base(options)
    {
    }

    public DbSet<Author> Authors => Set<Author>();

    public DbSet<Publisher> Publishers => Set<Publisher>();

    public DbSet<Genre> Genres => Set<Genre>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Book> Books => Set<Book>();

    public DbSet<BookGenre> BookGenres => Set<BookGenre>();

    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BookGenre>()
            .HasKey(bookGenre => new { bookGenre.BookId, bookGenre.GenreId });

        modelBuilder.Entity<Book>()
            .HasOne(book => book.Author)
            .WithMany(author => author.Books)
            .HasForeignKey(book => book.AuthorId);

        modelBuilder.Entity<Book>()
            .HasOne(book => book.Publisher)
            .WithMany(publisher => publisher.Books)
            .HasForeignKey(book => book.PublisherId);

        modelBuilder.Entity<BookGenre>()
            .HasOne(bookGenre => bookGenre.Book)
            .WithMany(book => book.BookGenres)
            .HasForeignKey(bookGenre => bookGenre.BookId);

        modelBuilder.Entity<BookGenre>()
            .HasOne(bookGenre => bookGenre.Genre)
            .WithMany(genre => genre.BookGenres)
            .HasForeignKey(bookGenre => bookGenre.GenreId);

        modelBuilder.Entity<Review>()
            .HasOne(review => review.Book)
            .WithMany(book => book.Reviews)
            .HasForeignKey(review => review.BookId);

        modelBuilder.Entity<Review>()
            .HasOne(review => review.User)
            .WithMany(user => user.Reviews)
            .HasForeignKey(review => review.UserId);
    }
}
