using MudBlazor;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Infrastructure.Web.UseCases;

namespace Musicx.Presentation.Web.Client.Pages.Users;

public partial class CreateAccount
{
    private readonly InUser _user = new()
    {
        EmailConfirmed = false
    };

    private bool _passwordMode = true;
    
    private readonly List<BreadcrumbItem> _breadcrumb =
    [
        new("Musicx", href: "/"),
        new("Users", href: "#"),
        new("Create Account", href: "#")
    ];

    private async Task Save()
    {
        await UcSave.ExecuteAsync(_user);
        Navigation.NavigateTo("/", forceLoad: true);
    }
}