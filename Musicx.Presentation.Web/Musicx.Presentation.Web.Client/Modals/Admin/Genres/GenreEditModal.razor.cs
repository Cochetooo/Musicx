using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Application.Shared.Enums;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Genre;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Genres;
using Musicx.Contracts.Enums;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Infrastructure.API.Persistence.Specifications.Genre;

namespace Musicx.Presentation.Web.Client.Modals.Admin.Genres;

public partial class GenreEditModal
{
    private ILogger _logger = null!;
    
    private InGenre _genre = new();

    private List<OutGenre> _genres = [];
    private List<OutGenre> _selectedParents = [];

    private MudDialog _modalRef = null!;
    private MudTextField<string> _nameTextEdit = null!;

    private GenreRelationType _genreType = GenreRelationType.IsA;

    private int _currentStep = 1;
    private GenreType? _selectedType;

    private bool _isNameAvailable = true;
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
    
    [Parameter] public EventCallback OnSave { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }

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
    
    public async Task Show(OutGenre? genre = null)
    {
        await _modalRef.ShowAsync();
        Clean();
        
        if (null != genre)
        {
            _genre = genre.ToRaw();
            await _nameTextEdit.SetText(genre.CanonicalName);
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task Hide()
        => await _modalRef.CloseAsync();

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
    }
    
    private void Clean()
    {
        _selectedParents.Clear();
        _genre = new InGenre();
        _currentStep = 1;
        _selectedType = null;
        _isNameAvailable = true;
        _derivedGradient = "transparent";
        _fusionPrefix = "";
        _fusionCustomizeName = false;
    }
}