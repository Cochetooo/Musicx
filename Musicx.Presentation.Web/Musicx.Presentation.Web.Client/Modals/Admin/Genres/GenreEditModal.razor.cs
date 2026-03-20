using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Application.Shared.Enums;
using Musicx.Contracts.Dto.Requests.Genre;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Genre;
using Musicx.Contracts.Dto.Responses.Specifics.Genres;
using Musicx.Contracts.Enums;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.API.Persistence.Specifications.Genre;
using Musicx.Presentation.Web.Client.Features.Genres.Editor;

namespace Musicx.Presentation.Web.Client.Modals.Admin.Genres;

public partial class GenreEditModal
{
    private ILogger _logger = null!;
    
    [Parameter] public EventCallback OnSave { get; set; }

    private MudDialog _modalRef = null!;
    
    private GenreNodeEditor _nodeEditor = null!;

    private readonly GenreEditState _state = new();

    private List<OutGenre> _rootGenres = [];

    protected override void OnInitialized()
    {
        _logger = LoggerFactory.CreateLogger(nameof(GenreEditModal));
    }

    public async Task Show(OutGenre? genre = null)
    {
        await EnsureGenreTreeLoadedAsync();
        
        Clean();
        await _modalRef.ShowAsync();

        if (genre is null)
        {
            await InvokeAsync(StateHasChanged);
            return;
        }
        
        var completeGenre = await UcFindOneGenre.ExecuteAsync(genre.Id, new GenreJoinSpecification
        {
            IncludeParents = true,
            IncludeRelations = true,
            IncludeAliases = true
        }) ?? genre;
        
        _state.Node = completeGenre.ToRaw();
        await InvokeAsync(StateHasChanged);
        await _nodeEditor.UpdateCanonicalName(_state.Node.CanonicalName);

        if (completeGenre.Relations is not null)
        {
            _state.Relations.AddRange(completeGenre.Relations.Select(r => r.Relation.ToRaw()));
        }
        
        if (completeGenre.Parents is not null)
        {
            _state.Relations.AddRange(completeGenre.Parents.Select(p => new InGenreRelation
            {
                Type = GenreRelationType.IsA,
                FromGenreId = p.Relation.Id,
                ToGenreId = completeGenre.Id,
                Weight = 1f
            }));
        }

        if (completeGenre.Aliases is not null)
        {
            _state.Aliases.AddRange(completeGenre.Aliases.Select(a => a.ToRaw()));
        }
    }

    private async Task HideAsync()
        => await _modalRef.CloseAsync();

    private void NextStep() => _state.Step = Math.Min(3, _state.Step + 1);
    private void PreviousStep() => _state.Step = Math.Max(1, _state.Step - 1);

    private async Task SaveAsync()
    {
        var result = await UcSaveGenre.ExecuteAsync(_state.Node);

        if (!result.IsSuccessStatusCode)
        {
            Snackbar.Add($"Could not save genre: {result.ReasonPhrase}", Severity.Error);
            await HideAsync();
            return;
        }

        var resultContent = await result.Content.ReadAsStringAsync();

        // If result doesn't give the id, then we stop and display an error.
        if (!long.TryParse(resultContent, out long genreId))
        {
            Snackbar.Add($"Unexpected result while saving genre (expecting long integer): {resultContent}",
                Severity.Error);
            await HideAsync();
            return;
        }

        // Set the new genre id to each relations.
        _state.Relations.ForEach(r => r.ToGenreId = genreId);

        result = await UcSaveRelations.ExecuteAsync(_state.Relations);

        if (!result.IsSuccessStatusCode)
        {
            Snackbar.Add($"Could not save genre relations: {result.ReasonPhrase}", Severity.Warning);
        }

        // Set the new genre id to each facets.
        var facets = _state.Facets.Select(f => new InGenreFacet
        {
            GenreId = genreId,
            FacetId = f.Id,
        }).ToList();

        result = await UcSaveFacets.ExecuteAsync(facets);

        if (!result.IsSuccessStatusCode)
        {
            Snackbar.Add($"Could not save genre facets: {result.ReasonPhrase}", Severity.Warning);
        }

        // Set the new genre id to each aliases.
        _state.Aliases.ForEach(a => a.GenreId = genreId);

        result = await UcSaveAliases.ExecuteAsync(_state.Aliases);

        if (!result.IsSuccessStatusCode)
        {
            Snackbar.Add($"Could not save genre aliases: {result.ReasonPhrase}", Severity.Warning);
        }
        
        Snackbar.Add("Genre saved successfully", Severity.Success);
        await OnSave.InvokeAsync();
        await HideAsync();
    }

    private async Task EnsureGenreTreeLoadedAsync()
    {
        _logger.LogInformation("🔄️ Loading all genres...");
        
        if (_rootGenres.Count > 0)
        {
            return;
        }
        
        var genres = await UcFindAllGenres.ExecuteAsync(
            joins: new GenreJoinSpecification
            {
                IncludeParents = true
            }, 
            pagingOptions: new PagingOptions(Take: 10_000, Skip: 0),
            order: new GenreOrderSpecification
            {
                CanonicalName = 1
            }
        );
        
        _rootGenres = genres
            .Where(g => g.Parents is null || g.Parents.Count < 2) // 2 because closure relationship contains the genre itself.
            .OrderBy(g => g.CanonicalName)
            .ToList();
        
        _logger.LogInformation("✅ Root genres loaded : " + _rootGenres.Count);
    }

    private async Task<IReadOnlyList<GenreClosureNode>> LoadChildrenAsync(OutGenre genre)
    {
        if (genre.Children is { Count: > 0 })
        {
            return genre.Children;
        }
        
        var hydratedGenre = await UcFindOneGenre.ExecuteAsync(genre.Id, joins: new GenreJoinSpecification
        {
            IncludeChildren = true
        });
        
        genre.Children = (hydratedGenre?.Children ?? [])
            .Where(c => c.Depth == 1)
            .OrderBy(c => c.Relation.CanonicalName)
            .ToList();
        
        return genre.Children;
    }

    private void Clean()
    {
        _state.Node = new InGenre
        {
            IsVisible = true,
            IsTaggable = true
        };
        
        _state.Relations.Clear();
        _state.Aliases.Clear();
        _state.Facets.Clear();
        _state.Step = 1;
        
        _nodeEditor.Clean();
    }
}