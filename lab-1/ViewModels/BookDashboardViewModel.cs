namespace Lab1.ViewModels;

public class BookDashboardViewModel
{
    public int TotalBooks { get; set; }

    public int TotalAuthors { get; set; }

    public int TotalUsers { get; set; }

    public double OverallAverageRating { get; set; }

    public List<BookCardViewModel> TopBooks { get; set; } = new();

    public List<ReviewCardViewModel> RecentReviews { get; set; } = new();

    public List<GenreStatViewModel> GenreStats { get; set; } = new();
}