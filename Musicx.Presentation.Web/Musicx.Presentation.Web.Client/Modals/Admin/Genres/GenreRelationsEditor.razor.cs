using Microsoft.AspNetCore.Components;
using Musicx.Contracts.Dto.Requests.Genre;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Enums;

namespace Musicx.Presentation.Web.Client.Modals.Admin.Genres;

public partial class GenreRelationsEditor
{
    [Parameter] public List<InGenreRelation> Relations { get; set; } = null!;
    [Parameter] public IReadOnlyList<OutGenre> RootGenres { get; set; } = null!;

    public void SetParents(IEnumerable<OutGenre> parents)
    {
        Relations.RemoveAll(r => r.Type == GenreRelationType.IsA);

        foreach (var p in parents)
        {
            Relations.Add(new InGenreRelation
            {
                Type = GenreRelationType.IsA,
                FromGenreId = p.Id,
                ToGenreId = 0
            });
        }
    }

    public void AddInfluence(OutGenre g)
    {
        Relations.Add(new InGenreRelation
        {
            Type = GenreRelationType.InfluencedBy,
            FromGenreId = g.Id,
            ToGenreId = 0
        });
    }
    
    public void AddFusionPart(OutGenre g, float weight)
    {
        Relations.Add(new InGenreRelation
        {
            Type = GenreRelationType.FusionOf,
            FromGenreId = g.Id,
            ToGenreId = 0,
            Weight = weight
        });
    }
}