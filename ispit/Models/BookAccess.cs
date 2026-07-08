using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab5.Models;

/// <summary>
/// Definiše koje knjige korisnik može vidjeti i koja je razina pristupa
/// </summary>
public class BookAccess
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string AppUserId { get; set; } = string.Empty;

    public virtual AppUser User { get; set; } = default!;

    [Required]
    public int BookId { get; set; }

    public virtual Book Book { get; set; } = default!;

    /// <summary>
    /// Razina pristupa: Read (samo čitanje), ReadWrite (čitanje i pisanje), Denied (blokiran pristup)
    /// </summary>
    [Required]
    [StringLength(50)]
    public string AccessLevel { get; set; } = "Read";

    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// ID admina koji je dao dozvolu (može biti null za sistemske dozvole)
    /// </summary>
    public string? GrantedBy { get; set; }

    [StringLength(500)]
    public string? Reason { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}

