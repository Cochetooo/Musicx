using MudBlazor;
using Musicx.Contracts.Dto.Requests.Specifics;

namespace Musicx.Presentation.Web.Client.Modals.Users;

public partial class LoginModal
{
    private string 
        _email = string.Empty, 
        _password = string.Empty;

    private MudDialog _modalRef = null!;

    private bool _passwordMode = true;

    private async Task Login()
    {
        var token = await UcSignIn.ExecuteAsync(new SignInRequest(_email, _password));
        
        await Hide();

        await UserClientContext.RefreshAsync();

        await InvokeAsync(StateHasChanged);
    }

    public async Task Show()
    {
        await _modalRef.ShowAsync();
        
        _email = string.Empty;
        _password = string.Empty;
    }

    private async Task Hide()
    {
        await _modalRef.CloseAsync();
    }
}