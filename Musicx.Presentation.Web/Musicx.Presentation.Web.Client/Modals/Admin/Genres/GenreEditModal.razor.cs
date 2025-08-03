using Blazorise;
using Microsoft.AspNetCore.Components;
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
    
    private bool _isConfirmable;

    private Modal _modalRef = null!;
    private TextEdit _nameTextEdit = null!;
    
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
        Hide();
        
        await Load();
        _selectedParents.Clear();
    }

    public void Show(OutGenre? genre = null)
    {
        if (null != genre)
        {
            _genre = genre.ToRaw();
            _nameTextEdit.Text = genre.Name;
            _nameTextEdit.Revalidate();
            StateHasChanged();
        }
        
        _modalRef.Show();
    }

    private void Hide()
    {
        _modalRef.Hide();
    }

    private void ValidateNonEmptyField(ValidatorEventArgs e)
    {
        _isConfirmable = string.IsNullOrWhiteSpace(Convert.ToString(e.Value));
        
        e.Status = _isConfirmable
            ? ValidationStatus.None
            : ValidationStatus.Success;
    }
    
    private void NameTextChanged(string newValue)
    {
        _genre.Name = newValue;
    }
}