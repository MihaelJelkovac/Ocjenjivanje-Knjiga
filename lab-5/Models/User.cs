using System.ComponentModel.DataAnnotations;

namespace Lab5.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Korisničko ime je obavezno")]
    [StringLength(100, MinimumLength = 3)]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Korisničko ime može sadržavati samo slova, brojeve i _")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Puno ime je obavezno")]
    [StringLength(200, MinimumLength = 3)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email je obavezan")]
    [EmailAddress(ErrorMessage = "Email nije u ispravnom formatu")]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    public DateTime JoinedAt { get; set; }

    [StringLength(100)]
    public string FavoriteGenre { get; set; } = string.Empty;

    [Range(0, 10000, ErrorMessage = "Reputacijski bodovi moraju biti između 0 i 10000")]
    public int ReputationPoints { get; set; }

    public bool IsPremiumMember { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public DateTime? DeletedAt { get; set; }
}

