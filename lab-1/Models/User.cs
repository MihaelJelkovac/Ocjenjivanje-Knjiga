namespace Lab1.Models;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime JoinedAt { get; set; }

    public string FavoriteGenre { get; set; } = string.Empty;

    public int ReputationPoints { get; set; }

    public bool IsPremiumMember { get; set; }

    public List<Review> Reviews { get; set; } = new();
}