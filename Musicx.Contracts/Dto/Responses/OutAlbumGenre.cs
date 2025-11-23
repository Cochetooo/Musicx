using System.Text.Json.Nodes;
using Musicx.Contracts.Enums;

namespace Musicx.Contracts.Dto.Responses;

public sealed class OutAlbumGenre : BaseOutputModel
{
    public OutAlbum Album { get; set; } = null!;
    public OutGenre Genre { get; set; } = null!;
    public OutUser Tagger { get; set; } = null!;

    public float Confidence { get; set; } = 0.8f;
    public JsonObject? Metadata { get; set; }
    public GenreVoteSource Source { get; set; }
}