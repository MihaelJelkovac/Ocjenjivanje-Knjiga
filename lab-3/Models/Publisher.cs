using System.ComponentModel.DataAnnotations;

namespace Lab3.Models;

public class Publisher
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public DateTime FoundedOn { get; set; }

    public string Website { get; set; } = string.Empty;

    public string ContactEmail { get; set; } = string.Empty;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
