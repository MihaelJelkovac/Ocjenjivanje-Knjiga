using System.ComponentModel.DataAnnotations;

namespace Lab5.Models;

public class Genre
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Naziv žanra je obavezan")]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [StringLength(100)]
    public string Audience { get; set; } = string.Empty;

    public virtual ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();

    public DateTime? DeletedAt { get; set; }
}

