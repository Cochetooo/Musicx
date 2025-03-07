using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Musicx.Data.Entities;

[Table("Labels")]
public class LabelEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong Id { get; set; }

    public string? Description { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    // Relation avec Album (un Label peut avoir plusieurs albums)
    public virtual List<AlbumEntity> Albums { get; set; } = new();
}
