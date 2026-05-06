using Lab3.Models;

namespace Lab3.ViewModels;

public class ReviewCardViewModel
{
    public string BookTitle { get; set; } = string.Empty;

    public string ReviewerName { get; set; } = string.Empty;

    public int Score { get; set; }

    public ReviewSentiment Sentiment { get; set; }

    public string Comment { get; set; } = string.Empty;

    public DateTime ReviewedAt { get; set; }
}
