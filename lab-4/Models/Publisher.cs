using System.ComponentModel.DataAnnotations;

namespace Lab4.Models;

public class Publisher
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Naziv izdavača je obavezan")]
    [StringLength(200, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Grad je obavezan")]
    [StringLength(100, MinimumLength = 2)]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Država je obavezna")]
    [StringLength(100, MinimumLength = 2)]
    public string Country { get; set; } = string.Empty;

    public DateTime FoundedOn { get; set; }

    [StringLength(500)]
    [Url]
    public string Website { get; set; } = string.Empty;

    [StringLength(150)]
    [EmailAddress]
    public string ContactEmail { get; set; } = string.Empty;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();

    public DateTime? DeletedAt { get; set; }
}
