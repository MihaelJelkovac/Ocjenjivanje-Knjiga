using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab4.Models;

public class Book
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Naslov knjige je obavezan")]
    [StringLength(500, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "ISBN je obavezan")]
    [StringLength(20, MinimumLength = 10)]
    [Isbn(ErrorMessage = "ISBN nije u ispravnom formatu")]
    public string Isbn { get; set; } = string.Empty;

    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    public DateTime PublishedOn { get; set; }

    [Range(1, 10000, ErrorMessage = "Broj stranica mora biti između 1 i 10000")]
    public int PageCount { get; set; }

    [StringLength(50)]
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

    public DateTime? DeletedAt { get; set; }
}
