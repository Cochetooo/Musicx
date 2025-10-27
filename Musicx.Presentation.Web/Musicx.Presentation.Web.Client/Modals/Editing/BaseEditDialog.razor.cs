using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Contracts.Dto.Requests;

namespace Musicx.Presentation.Web.Client.Modals.Editing;

public partial class BaseEditDialog<T> where T : BaseInputModel, new()
{
    [Parameter] public string Title { get; set; } = "Edit";
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public BaseEditDialogLogic<T> Logic { get; set; } = null!;
    [Parameter] public EventCallback OnClose { get; set; }

    private MudDialog _dialogRef = null!;

    public async Task ShowAsync()
    {
        await _dialogRef.ShowAsync();
    }

    private async Task HideAsync()
    {
        await _dialogRef.CloseAsync();
        await OnClose.InvokeAsync();
    }

    private async Task OnSaveClicked()
    {
        // Appelle la méthode Save définie dans la logique spécifique
        //await Logic.SaveAsync(async (entity) =>
        //{
            // On délègue la vraie persistance au composant parent
            //return await OnSaveRequested.InvokeAsync(entity);
        //});
    }

    [Parameter] public EventCallback<T> OnSaveRequested { get; set; }
}