using System.ComponentModel.DataAnnotations;

namespace Lab3.Models;

public class Genre
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public virtual ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
}
