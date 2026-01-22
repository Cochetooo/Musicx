using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Contracts.Dto.Requests.Genre;
using Musicx.Contracts.Dto.Responses;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Presentation.Web.Client.Features.Genres.Editor;

namespace Musicx.Presentation.Web.Client.Modals.Admin.Genres;

public partial class GenreEditModal
{
    [Parameter] public EventCallback OnSave { get; set; }
    
    private ILogger _logger = null!;

    private MudDialog _modalRef = null!;

    private GenreTypeSelector _typeSelector = null!;
    private GenreNodeEditor _nodeEditor = null!;
    private GenreRelationsEditor _relationsEditor = null!;

    private readonly GenreEditState _state = new();

    public async Task Show(OutGenre? genre = null)
    {
        await _modalRef.ShowAsync();
        Clean();

        if (null != genre)
        {
            _state.Node = genre.ToRaw();
            await _nodeEditor.UpdateCanonicalName(genre.CanonicalName);
        }
    }

    private async Task HideAsync()
        => await _modalRef.CloseAsync();

    private void NextStep() => _state.Step++;
    private void PreviousStep() => _state.Step--;

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
        
        await OnSave.InvokeAsync();
    }

    private void Clean()
    {
        _state.Node = new InGenre();
        _state.Relations.Clear();
        _state.Aliases.Clear();
        _state.Facets.Clear();
        
        _nodeEditor.Clean();
        
        //_derivedGradient = "transparent";
        //_fusionPrefix = "";
        //_fusionCustomizeName = false;
    }
}

/* private InGenre _genre = new();

private List<OutGenre> _genres = [];
private List<OutGenre> _selectedParents = [];

private GenreRelationType _genreType = GenreRelationType.IsA;

private GenreType? _selectedType;


private string _derivedGradient = "transparent";

// @TODO change properly
private static readonly string[] FusionPrefixes =
[
    "Atmospheric", "Symphonic", "Progressive", "Alternative", "Ambient", "Melodic"
];

private string? _fusionPrefix = "";
private bool _fusionCustomizeName = false;

private int? _selectedDescriptorId = null;
private List<OutGenre> _descriptors = [];

protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (!firstRender)
    {
        return;
    }

    _logger = LoggerProvider.CreateLogger(nameof(GenreEditModal));
    await Load();
}

private async Task Load()
{
    var result = await UcList.ExecuteAsync(
        joins: new GenreJoinSpecification
        {
            IncludeParents = true
        },
        pagingOptions: new PagingOptions(100_000, 0));

    _genres = result
        .Where(g => g.Parents?.Count > 0)
        .ToList();

    _descriptors = result
        .Where(d => d is { Type: GenreType.Descriptor, Parents.Count: > 0 })
        .ToList();

    _logger.LogInformation("✅ Genre list loaded successfully !");
}

private void SelectType(GenreType type)
{
    _selectedType = type;
    _genre = new InGenre { Type = type };
}

private void NextStep()
{
    if (_selectedType is null)
    {
        return;
    }

    _currentStep = 2;
    StateHasChanged();
}

private void PreviousStep()
{
    _currentStep = Math.Max(1, _currentStep - 1);
}

private string GetStepClass(int step)
{
    return step == _currentStep ? "step step-active" : "step step-inactive";
}

private async void NameTextChanged(string newValue)
{
    _genre.CanonicalName = newValue;
    await ValidateNameExists(newValue);
}

private CancellationTokenSource? _nameCts;
private async Task ValidateNameExists(string name)
{
    _nameCts?.Cancel();
    _nameCts = new CancellationTokenSource();
    var token = _nameCts.Token;

    try
    {
        await Task.Delay(300, token);
    }
    catch (TaskCanceledException)
    {
        return;
    }

    if (string.IsNullOrWhiteSpace(name))
    {
        _isNameAvailable = false;
        await InvokeAsync(StateHasChanged);
        return;
    }

    try
    {
        // @TODO
        _isNameAvailable = true;
    }
    catch (Exception ex)
    {
        _logger.LogWarning("⚠️ Error validating genre name: " + ex.Message);
        _isNameAvailable = false;
    }

    await InvokeAsync(StateHasChanged);
}

private async Task SelectedGenresChanged(IReadOnlyList<OutGenre> obj)
{
    _selectedParents.Clear();
    _selectedParents.AddRange(obj);
    ComputeDerivedGradient();

    if (_selectedType == GenreType.Fusion && !_fusionCustomizeName)
    {
        BuildFusionName();
        await ValidateNameExists(_derivedGradient);
    }

    await InvokeAsync(StateHasChanged);
}

private void ComputeDerivedGradient()
{
    if (_selectedParents.Count == 0)
    {
        _derivedGradient = "transparent";
        return;
    }

    // take parent.Color (hex) list; fallback to #cccccc
    var colors = _selectedParents.Select(p => string.IsNullOrWhiteSpace(p.Color) ? "#cccccc" : p.Color).ToList();
    // build linear-gradient
    _derivedGradient = $"linear-gradient(90deg, {string.Join(", ", colors)})";

    // set _genre.Color to null for subgenres (server-side should compute/accept gradient or special handling)
    _genre.Color = null;
}

private void BuildFusionName()
{
    var genreNames = _selectedParents.Select(g => g.CanonicalName).ToList();
    var joined = string.Join(" ", genreNames);
    if (!string.IsNullOrWhiteSpace(_fusionPrefix))
    {
        _genre.CanonicalName = $"{_fusionPrefix} {joined}".Trim();
    }
    else
    {
        _genre.CanonicalName = joined;
    }
}

private bool CanSave
{
    get
    {
        if (_selectedType is null) return false;
        if (string.IsNullOrWhiteSpace(_genre.CanonicalName)) return false;
        if (!_isNameAvailable) return false;
        // pour Subgenre, require at least one parent
        if (_selectedType == GenreType.Subgenre && !_selectedParents.Any()) return false;
        // autres règles métier possibles...
        return true;
    }
}

private async Task Save()
{
    var response = await UcSave.ExecuteAsync(_genre);

    if (!response.IsSuccessStatusCode)
    {
        Snackbar.Add($"Could not save genre: {response.ReasonPhrase}", Severity.Error);
        await Hide();
        return;
    }

    response = await UcSaveRelations.ExecuteAsync(_selectedParents.Select(pg => new InGenreRelation
    {
        FromGenreId = pg.Id,
        ToGenreId = _genre.Id,
        Type = _genreType,
        Weight = 0.8f
    }));

    Snackbar.Add("Genre saved successfully !", Severity.Success);

    if (!response.IsSuccessStatusCode)
    {
        Snackbar.Add($"Could not save genre relations: {response.ReasonPhrase}", Severity.Error);
        await Hide();
        return;
    }

    Snackbar.Add("Genre relations saved successfully !", Severity.Success);

    await OnSave.InvokeAsync();
    await Hide();

    await Load();
}

private async Task<IReadOnlyList<GenreClosureNode>> LoadChildrenAsync(OutGenre genre)
{
    _logger.LogInformation($"🔄️ Loading Server Data for : {genre.CanonicalName} | Array count: {genre.Children?.Count}");

    if (genre.Children is null)
    {
        genre = await UcGet.ExecuteAsync(genre.Id, new GenreJoinSpecification
        {
            IncludeChildren = true
        }) ?? genre;

        if (genre.Children is null)
        {
            return [];
        }
    }

    return genre.Children;
}

private IReadOnlyCollection<GenreType> AllowedFilterFor(GenreType type)
{
    return type switch
    {
        GenreType.Genre => new []{ GenreType.Subgenre, GenreType.Genre },
        GenreType.Subgenre => new []{ GenreType.Subgenre, GenreType.Genre },
        GenreType.Fusion => new []{ GenreType.Subgenre, GenreType.Genre, GenreType.Fusion },
        GenreType.Scene => new []{ GenreType.Subgenre, GenreType.Genre, GenreType.Fusion, GenreType.Scene },
        GenreType.Movement => new []{ GenreType.Subgenre, GenreType.Genre, GenreType.Fusion, GenreType.Movement },
        GenreType.Descriptor => new []{ GenreType.Descriptor },
        _ => []
    };
} */