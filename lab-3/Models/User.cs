using System.ComponentModel.DataAnnotations;

namespace Lab3.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime JoinedAt { get; set; }

    public string FavoriteGenre { get; set; } = string.Empty;

    public int ReputationPoints { get; set; }

    public bool IsPremiumMember { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
