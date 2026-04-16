namespace Lab1.Models;

public class Genre
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public List<BookGenre> BookGenres { get; set; } = new();
}