using Musicx.Contracts.Dto.Responses.Genre;

namespace Musicx.Contracts.Dto.Responses;

public sealed class OutGenreClosure : BaseOutputModel
{
    public OutGenre Ancestor { get; set; } = null!;
    public OutGenre Descendant { get; set; } = null!;
    
    public int Depth { get; set; }
}