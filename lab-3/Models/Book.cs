using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab3.Models;

public class Book
{
    [Key]
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Isbn { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime PublishedOn { get; set; }

    public int PageCount { get; set; }

    public string Language { get; set; } = string.Empty;

    public BookStatus Status { get; set; }

    [ForeignKey(nameof(Author))]
    public int AuthorId { get; set; }

    public virtual Author Author { get; set; } = default!;

    [ForeignKey(nameof(Publisher))]
    public int PublisherId { get; set; }

    public virtual Publisher Publisher { get; set; } = default!;

    public virtual ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
