using Lab1.Models;

namespace Lab1.ViewModels;

public class BookCardViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string AuthorName { get; set; } = string.Empty;

    public string PublisherName { get; set; } = string.Empty;

    public string Genres { get; set; } = string.Empty;

    public BookStatus Status { get; set; }

    public DateTime PublishedOn { get; set; }

    public double AverageRating { get; set; }

    public int ReviewCount { get; set; }
}