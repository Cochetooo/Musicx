using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Contracts.Dto.Requests.Specifics;

namespace Musicx.Presentation.Web.Client.Modals.Users;

public partial class LoginModal
{
    [Parameter] public EventCallback<string> OnLogin { get; set; }
    
    private string 
        _email = string.Empty, 
        _password = string.Empty;

    private MudDialog _modalRef = null!;

    private bool _passwordMode = true;

    private async Task Login()
    {
        var response = await UcSignIn.ExecuteAsync(new SignInRequest(_email, _password));
        
        await OnLogin.InvokeAsync(response);
        await Hide();
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