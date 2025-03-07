using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Musicx.Data.Entities;

[Table("Genres")]
public class GenreEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    // Relation Parent-Genres (auto-référence Many-to-Many)
    public virtual List<GenreParentEntity> Parents { get; set; } = new();
    public virtual List<GenreParentEntity> Children { get; set; } = new();
}

[Table("GenreParents")]
[PrimaryKey("ParentId", "ChildId")]
public class GenreParentEntity
{
    public ulong ParentId { get; set; }
    [ForeignKey(nameof(ParentId))]
    public virtual GenreEntity Parent { get; set; } = null!;
    
    public ulong ChildId { get; set; }
    [ForeignKey(nameof(ChildId))]
    public virtual GenreEntity Child { get; set; } = null!;
}