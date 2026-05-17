using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab4.Models;

public class Review
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Ocjena je obavezna")]
    [Range(1, 5, ErrorMessage = "Ocjena mora biti između 1 i 5")]
    public int Score { get; set; }

    [Required(ErrorMessage = "Naslov recenzije je obavezan")]
    [StringLength(300, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
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

    public DateTime? DeletedAt { get; set; }
}
