using System.Net;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Musicx.Presentation.Web.Client.Modals.Searches;
using Musicx.Presentation.Web.Client.Modals.Users;

namespace Musicx.Presentation.Web.Client.Layout;

public partial class MainLayout
{
    private ILogger _logger = null!;
    
    private bool _isDarkMode;
    private MudThemeProvider _mudThemeProvider = null!;

    private string _searchText = string.Empty;

    private MudTextField<string> _navSearchRef;
    private LoginModal _loginModal = null!;
    private SearchOverlay _searchOverlay = null!;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        _isDarkMode = await _mudThemeProvider.GetSystemDarkModeAsync();
        _logger = LoggerFactory.CreateLogger(nameof(MainLayout));

        await UserClientContext.RefreshAsync();

        await InvokeAsync(StateHasChanged);
    }

    private async Task OpenSearch(FocusEventArgs e)
    {
        await _searchOverlay.OpenFromNavAsync(".nav-search-field input");
    }

    private void SearchKeyDown(KeyboardEventArgs keyEvent)
    {
        if (keyEvent.Key == "Enter")
        {
            _ = _searchOverlay.OpenFromNavAsync(".nav-search-field input");
        }
    }

    private Task OnOverlayClose(string finalValue)
    {
        _searchText = finalValue;
        StateHasChanged();
        return Task.CompletedTask;
    }

    private void Search()
        => NavigationManager.NavigateTo($"/SearchResult/{WebUtility.UrlEncode(_searchText)}");

    private void NavigateHome()
        => NavigationManager.NavigateTo("/");

    private async Task SignOut()
    {
        await Http.PostAsync("/api/auth/signout", null);

        await UserClientContext.RefreshAsync();
    }
}