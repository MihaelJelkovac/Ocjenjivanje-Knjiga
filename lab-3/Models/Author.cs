using System.ComponentModel.DataAnnotations;

namespace Lab3.Models;

public class Author
{
    [Key]
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Biography { get; set; } = string.Empty;

    public DateTime BirthDate { get; set; }

    public string Nationality { get; set; } = string.Empty;

    public string Website { get; set; } = string.Empty;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
