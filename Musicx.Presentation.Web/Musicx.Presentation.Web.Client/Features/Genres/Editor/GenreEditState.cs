using Musicx.Contracts.Dto.Requests.Genre;

namespace Musicx.Presentation.Web.Client.Features.Genres.Editor;

public sealed class GenreEditState
{
    public InGenre Node { get; set; } = new();

    public List<InGenreRelation> Relations { get; } = new();
    public List<InFacet> Facets { get; } = new();
    public List<InGenreAlias> Aliases { get; } = new();

    public int Step { get; set; } = 1;
}