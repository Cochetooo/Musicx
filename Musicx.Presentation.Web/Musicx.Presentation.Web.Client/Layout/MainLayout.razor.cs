using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Musicx.Contracts.Enums;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Musicx.Presentation.Web.Client.Modals.Searches;
using Musicx.Presentation.Web.Client.Modals.Users;

namespace Musicx.Presentation.Web.Client.Layout;

public partial class MainLayout
{
    private ILogger _logger = null!;
    
    private bool _isDarkMode;
    private MudThemeProvider _mudThemeProvider = null!;

    private LoginModal _loginModal = null!;
    private SearchOverlay _searchOverlay = null!;
    
    private readonly List<(string Code, string Label, string CountryName)> _languages =
    [
        ("en", "English", "United States"),
        ("fr", "Français", "France")
    ];
    
    private readonly List<RatingMode> _quickRatingModes =
    [
        RatingMode.OutOfFive,
        RatingMode.OutOfTen,
        RatingMode.OutOfTwenty,
        RatingMode.Percentage,
        RatingMode.RatingStars,
        RatingMode.TextualDetailed,
        RatingMode.TierListDetailed
    ];

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }
        
        _logger = LoggerFactory.CreateLogger(nameof(MainLayout));
        
        
        await UserClientContext.RefreshAsync();
        _isDarkMode = UserClientContext.CurrentUser?.PrefDarkMode ?? await _mudThemeProvider.GetSystemDarkModeAsync();
        ApplyUserLanguagePreference();

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

    private void NavigateHome()
        => NavigationManager.NavigateTo("/");

    private async Task OpenLogin()
    {
        await _loginModal.Show();
        await InvokeAsync(StateHasChanged);
    }

    private async Task HandleLogin(string response)
    {
        if (response.Contains("Invalid credentials"))
        {
            Snackbar.Add(response, Severity.Error);
        }
        else
        {
            Snackbar.Add("You are now connected!", Severity.Success);
        }

        await UserClientContext.RefreshAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task SignOut()
    {
        await Http.PostAsync("/api/auth/signout", null);

        await UserClientContext.RefreshAsync();
    }
    
    private void ApplyUserLanguagePreference()
    {
        var pref = UserClientContext.CurrentUser?.PrefLanguage;
        _logger.LogInformation("🗣️ User Language is : " + pref);
        
        if (string.IsNullOrWhiteSpace(pref))
        {
            return;
        }

        T.SetCulture(new CultureInfo(pref));
    }
    
    private async Task SaveCurrentUserPreferences()
    {
        if (UserClientContext.CurrentUser is null)
        {
            return;
        }
        
        var rawUser = UserClientContext.CurrentUser.ToRaw();
        await UcSaveUser.ExecuteAsync(rawUser);
    }

    private async Task ChangeRatingMode(RatingMode mode)
    {
        if (UserClientContext.CurrentUser is null)
        {
            return;
        }

        UserClientContext.CurrentUser.PrefRatingMode = mode;
        await SaveCurrentUserPreferences();
        await InvokeAsync(StateHasChanged);
    }
    
    private async Task ToggleRatings(bool value)
    {
        if (UserClientContext.CurrentUser is null)
        {
            return;
        }

        UserClientContext.CurrentUser.PrefShowRatings = value;
        await SaveCurrentUserPreferences();
        await InvokeAsync(StateHasChanged);
    }
    
    private async Task ChangeLanguage(string languageCode)
    {
        if (UserClientContext.CurrentUser is null)
        {
            return;
        }

        UserClientContext.CurrentUser.PrefLanguage = languageCode;
        ApplyUserLanguagePreference();
        await SaveCurrentUserPreferences();
        await InvokeAsync(StateHasChanged);
    }
}