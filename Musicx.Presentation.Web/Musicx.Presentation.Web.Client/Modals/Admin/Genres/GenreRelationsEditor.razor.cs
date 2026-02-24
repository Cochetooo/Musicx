using Microsoft.AspNetCore.Components;
using Musicx.Contracts.Dto.Requests.Genre;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Genre;
using Musicx.Contracts.Dto.Responses.Specifics.Genres;
using Musicx.Contracts.Enums;

namespace Musicx.Presentation.Web.Client.Modals.Admin.Genres;

public partial class GenreRelationsEditor
{
    private static readonly IReadOnlyCollection<GenreType> AllowedParentTypes =
    [
        GenreType.Genre,
        GenreType.Subgenre,
        GenreType.Scene,
        GenreType.Movement,
        GenreType.Localization,
        GenreType.Fusion
    ];
    
    private static readonly IReadOnlyCollection<GenreType> AllowedFusionTypes =
        [GenreType.Genre, GenreType.Subgenre, GenreType.Fusion, GenreType.Scene, GenreType.Movement, GenreType.Localization];

    [Parameter] public List<InGenreRelation> Relations { get; set; } = [];
    [Parameter] public IReadOnlyList<OutGenre> RootGenres { get; set; } = [];
    [Parameter] public Func<OutGenre, Task<IReadOnlyList<GenreClosureNode>>>? LoadChildrenAsync { get; set; }

    private IEnumerable<InGenreRelation> ParentRelations => Relations.Where(r => r.Type == GenreRelationType.IsA);
    private IEnumerable<InGenreRelation> InfluenceRelations => Relations.Where(r => r.Type == GenreRelationType.InfluencedBy);
    private IEnumerable<InGenreRelation> FusionRelations => Relations.Where(r => r.Type == GenreRelationType.FusionOf);

    private void OnParentsChanged(IReadOnlyList<OutGenre> genres)
    {
        Relations.AddRange(genres.Select(g => new InGenreRelation
        {
            Type = GenreRelationType.IsA,
            FromGenreId = g.Id,
            ToGenreId = 0,
            Weight = 1f
        }));
    }

    private void OnInfluencesChanged(IReadOnlyList<OutGenre> genres)
    {
        AddUnique(genres, GenreRelationType.InfluencedBy, 0.75f);
    }

    private void OnFusionChanged(IReadOnlyList<OutGenre> genres)
    {
        AddUnique(genres, GenreRelationType.FusionOf, 0.5f);
    }

    private void AddUnique(IEnumerable<OutGenre> genres, GenreRelationType type, float defaultWeight)
    {
        foreach (var genre in genres)
        {
            if (Relations.Any(r => r.Type == type && r.FromGenreId == genre.Id))
            {
                continue;
            }
            
            Relations.Add(new InGenreRelation
            {
                Type = type,
                FromGenreId = genre.Id,
                ToGenreId = 0,
                Weight = defaultWeight
            });
        }
    }

    private OutGenre? FindGenre(long id)
    {
        if (id <= 0)
        {
            return null;
        }
        
        return FindGenreInTree(RootGenres, id);
    }

    private static OutGenre? FindGenreInTree(IEnumerable<OutGenre> nodes, long id)
    {
        foreach (var node in nodes)
        {
            if (node.Id == id)
            {
                return node;
            }

            var children = node.Children?.Select(c => c.Relation) ?? [];
            var childMatch = FindGenreInTree(children, id);
            if (childMatch is not null)
            {
                return childMatch;
            }
        }
        
        return null;
    }
    
    private void RemoveRelation(InGenreRelation relation)
    {
        Relations.Remove(relation);
    }
}