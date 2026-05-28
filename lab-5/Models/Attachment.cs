using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab5.Models;

public class Attachment
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Book))]
    public int BookId { get; set; }

    public virtual Book Book { get; set; } = default!;

    [StringLength(255)]
    public string FileName { get; set; } = string.Empty;

    [StringLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [StringLength(150)]
    public string ContentType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public DateTime CreatedAt { get; set; }
}