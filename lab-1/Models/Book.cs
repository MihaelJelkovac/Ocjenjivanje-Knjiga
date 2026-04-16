namespace Lab1.Models;

public class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Isbn { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime PublishedOn { get; set; }

    public int PageCount { get; set; }

    public string Language { get; set; } = string.Empty;

    public BookStatus Status { get; set; }

    public int AuthorId { get; set; }

    public Author Author { get; set; } = default!;

    public int PublisherId { get; set; }

    public Publisher Publisher { get; set; } = default!;

    public List<BookGenre> BookGenres { get; set; } = new();

    public List<Review> Reviews { get; set; } = new();
}