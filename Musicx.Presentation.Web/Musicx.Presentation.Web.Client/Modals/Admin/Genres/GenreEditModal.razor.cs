using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
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
        
        _logger.LogInformation("✅ Genre list loaded successfully !");
    }

    private async Task Save()
    {
        _genre.ParentIds = _selectedParents.Select(g => g.Id).ToList();
        await UcSave.ExecuteAsync(_genre);
        await OnSave.InvokeAsync();
        await Hide();
        
        await Load();
    }

    public async Task Show(OutGenre? genre = null)
    {
        await _modalRef.ShowAsync();

        Clean();
        
        if (null != genre)
        {
            _genre = genre.ToRaw();
            await _nameTextEdit.SetText(genre.Name);
            // _nameTextEdit.Revalidate();
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task Hide()
    {
        await _modalRef.CloseAsync();
    }

    /* private void ValidateNonEmptyField(ValidatorEventArgs e)
    {
        _isConfirmable = string.IsNullOrWhiteSpace(Convert.ToString(e.Value));
        
        e.Status = _isConfirmable
            ? ValidationStatus.None
            : ValidationStatus.Success;
    } */
    
    private async Task<IReadOnlyList<OutGenre>> LoadChildrenAsync(OutGenre genre)
    {
        _logger.LogInformation($"🔄️ Loading Server Data for : {genre.Name} | Array count: {genre.Children?.Count}");
        
        if (genre.Children is null)
        {
            return [];
        }
        
        return await UcFindIn.ExecuteAsync(genre.Children.Select(g => g.Id), "children");
    }
    
    private void NameTextChanged(string newValue)
    {
        _genre.Name = newValue;
    }
    
    private void SelectedGenresChanged(IReadOnlyList<OutGenre> obj)
    {
        _selectedParents.Clear();
        _selectedParents.AddRange(obj);
    }

    private void Clean()
    {
        _selectedParents.Clear();
        _genre = new InGenre();
    }
}