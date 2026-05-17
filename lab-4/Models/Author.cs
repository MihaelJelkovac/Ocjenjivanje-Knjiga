using System.ComponentModel.DataAnnotations;

namespace Lab4.Models;

public class Author
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Ime autora je obavezno")]
    [StringLength(100, MinimumLength = 2)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Prezime autora je obavezno")]
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

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();

    public DateTime? DeletedAt { get; set; }
}
