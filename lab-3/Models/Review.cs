using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab3.Models;

public class Review
{
    [Key]
    public int Id { get; set; }

    public int Score { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Comment { get; set; } = string.Empty;

    public DateTime ReviewedAt { get; set; }

    public bool IsRecommended { get; set; }

    public ReviewSentiment Sentiment { get; set; }

    [ForeignKey(nameof(Book))]
    public int BookId { get; set; }

    public virtual Book Book { get; set; } = default!;

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    public virtual User User { get; set; } = default!;
}
