using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Contracts.Dto.Requests.Genre;

namespace Musicx.Presentation.Web.Client.Modals.Admin.Genres;

public partial class GenreNodeEditor
{
    [Parameter] public InGenre Node { get; set; } = null!;
    
    private ILogger _logger = null!;
    
    private bool _isNameAvailable = true;
    private MudTextField<string> _nameTextEdit = null!;

    protected override void OnInitialized()
    {
        _logger = LoggerFactory.CreateLogger(nameof(GenreNodeEditor));
    }

    public async Task UpdateCanonicalName(string newValue)
    {
        await _nameTextEdit.SetTextAsync(newValue);
        await InvokeAsync(StateHasChanged);
    }

    private async void NameTextChanged(string? newValue)
    {
        if (string.IsNullOrWhiteSpace(newValue) || newValue == Node.CanonicalName)
        {
            return;
        }
        
        Node.CanonicalName = newValue;
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

    public void Clean()
    {
        _isNameAvailable = true;
    }
}