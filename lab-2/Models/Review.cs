namespace Lab2.Models;

public class Review
{
    public int Id { get; set; }

    public int Score { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Comment { get; set; } = string.Empty;

    public DateTime ReviewedAt { get; set; }

    public bool IsRecommended { get; set; }

    public ReviewSentiment Sentiment { get; set; }

    public int BookId { get; set; }

    public Book Book { get; set; } = default!;

    public int UserId { get; set; }

    public User User { get; set; } = default!;
}