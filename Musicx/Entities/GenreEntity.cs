using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Musicx.Entities;

[Table("Genres")]
public class GenreEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong Id { get; set; }

    [Required]
    public required string Name { get; set; }

    // Relation Parent-Genres (auto-référence Many-to-Many)
    public virtual List<GenreParentEntity> Parents { get; set; } = new();
    public virtual List<GenreParentEntity> Children { get; set; } = new();
}

[Table("GenreParents")]
public class GenreParentEntity
{
    [Key]
    public ulong ParentId { get; set; }
    [ForeignKey(nameof(ParentId))]
    public virtual GenreEntity Parent { get; set; } = null!;

    [Key]
    public ulong ChildId { get; set; }
    [ForeignKey(nameof(ChildId))]
    public virtual GenreEntity Child { get; set; } = null!;
}