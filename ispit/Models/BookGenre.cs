using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab5.Models;

public class BookGenre
{
    [ForeignKey(nameof(Book))]
    public int BookId { get; set; }

    public virtual Book Book { get; set; } = default!;

    [ForeignKey(nameof(Genre))]
    public int GenreId { get; set; }

    public virtual Genre Genre { get; set; } = default!;

    public DateTime AddedAt { get; set; }
}

