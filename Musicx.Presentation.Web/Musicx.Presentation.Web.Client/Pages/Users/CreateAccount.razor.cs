using MudBlazor;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.User;

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
        var response = await Api.SaveAsync(_user);
        
        if (response.IsSuccessStatusCode)
        {
            Snackbar.Add("New user created successfully!", Severity.Success);
            Navigation.NavigateTo("/", forceLoad: true);
        }
        else
        {
            Snackbar.Add($"Error while signing up : {response.ReasonPhrase}", Severity.Error);
            _user.Password = null;
        }
    }
}