namespace Lab1.Models;

public class Publisher
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public DateTime FoundedOn { get; set; }

    public string Website { get; set; } = string.Empty;

    public string ContactEmail { get; set; } = string.Empty;

    public List<Book> Books { get; set; } = new();
}