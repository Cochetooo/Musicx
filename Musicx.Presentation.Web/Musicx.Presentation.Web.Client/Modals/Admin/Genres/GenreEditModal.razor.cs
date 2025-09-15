using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Enums;
using Musicx.Infrastructure.API.Persistence.Mappers;

namespace Musicx.Presentation.Web.Client.Modals.Admin.Genres;

public partial class GenreEditModal
{
    private ILogger _logger = null!;
    
    private InGenre _genre = new();

    private List<OutGenre> _genres = [];
    private List<OutGenre> _selectedParents = [];

    private MudDialog _modalRef = null!;
    private MudTextField<string> _nameTextEdit = null!;

    private int _currentStep = 1;
    private GenreType? _selectedType = null;

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
        var result = await UcList.ExecuteAsync(take: 10_000, query: "parents_children");
        
        _genres = result
            .Where(g => g.Parents is null)
            .ToList();

        _descriptors = result
            .Where(d => d.Type == GenreType.Descriptor && d.Parents is null)
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
            await _nameTextEdit.SetText(genre.Name);
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
        _genre.Name = newValue;
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
        var genreNames = _selectedParents.Select(g => g.Name).ToList();
        var joined = string.Join(" ", genreNames);
        if (!string.IsNullOrWhiteSpace(_fusionPrefix))
        {
            _genre.Name = $"{_fusionPrefix} {joined}".Trim();
        }
        else
        {
            _genre.Name = joined;
        }
    }
    
    private bool CanSave
    {
        get
        {
            if (_selectedType is null) return false;
            if (string.IsNullOrWhiteSpace(_genre.Name)) return false;
            if (!_isNameAvailable) return false;
            // pour Subgenre, require at least one parent
            if (_selectedType == GenreType.Subgenre && !_selectedParents.Any()) return false;
            // autres règles métier possibles...
            return true;
        }
    }
    
    private async Task Save()
    {
        _genre.ParentIds = _selectedParents.Select(g => g.Id).ToList();
        await UcSave.ExecuteAsync(_genre);
        await OnSave.InvokeAsync();
        await Hide();
        
        await Load();
    }
    
    private async Task<IReadOnlyList<OutGenre>> LoadChildrenAsync(OutGenre genre)
    {
        _logger.LogInformation($"🔄️ Loading Server Data for : {genre.Name} | Array count: {genre.Children?.Count}");
        
        if (genre.Children is null)
        {
            return [];
        }
        
        return await UcFindIn.ExecuteAsync(genre.Children.Select(g => g.Id), "children");
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