using System.Text.Json.Nodes;
using Musicx.Contracts.Dto.Responses.Genre;
using Musicx.Contracts.Enums;

namespace Musicx.Contracts.Dto.Responses;

public sealed class OutAlbumInfluence : BaseOutputModel
{
    public OutAlbum Album { get; set; } = null!;
    public OutGenre Genre { get; set; } = null!;
    public OutUser Tagger { get; set; } = null!;

    public float Confidence { get; set; } = 0.8f;
    public String? Metadata { get; set; }
    public GenreVoteSource Source { get; set; }
}