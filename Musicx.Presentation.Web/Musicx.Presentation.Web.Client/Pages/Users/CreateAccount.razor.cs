using MudBlazor;
using Musicx.Contracts.Dto.Requests;
using Musicx.Infrastructure.Web.UseCases;

namespace Musicx.Presentation.Web.Client.Pages.Users;

public partial class CreateAccount
{
    private readonly InUser _user = new()
    {
        EmailConfirmed = false
    };

    private bool _isPasswordVisible = false;
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
    
    private readonly List<BreadcrumbItem> _breadcrumb =
    [
        new("Musicx", href: "/"),
        new("Users", href: "#"),
        new("Create Account", href: "#")
    ];

    private void TogglePasswordVisibility()
    {
        if (_isPasswordVisible)
        {
            _isPasswordVisible = false;
            _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
            _passwordInput = InputType.Password;
        }
        else
        {
            _isPasswordVisible = true;
            _passwordInputIcon = Icons.Material.Filled.Visibility;
            _passwordInput = InputType.Text;
        }
    }

    private async Task Save()
    {
        await UcSave.ExecuteAsync(_user);
        //Navigation.NavigateTo("/", forceLoad: true);
    }
}