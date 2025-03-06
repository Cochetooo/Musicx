using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Musicx.Entities;

[Table("Labels")]
public class LabelEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong Id { get; set; }

    public string? Description { get; set; }

    [Required]
    public required string Name { get; set; }

    // Relation avec Album (un Label peut avoir plusieurs albums)
    public virtual List<AlbumEntity> Albums { get; set; } = new();
}
