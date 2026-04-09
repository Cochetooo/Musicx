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
        ("de", "Deutsch", "Germany"),
        ("en", "English", "United States"),
        ("fr", "Français", "France"),
        ("it", "Italiano", "Italy")
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
    
    private string GraphicsModeClass =>
        UserClientContext.CurrentUser?.PrefHighGraphics ?? true
            ? "graphics-high"
            : "graphics-low";

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
        var pref = UserClientContext.CurrentUser?.PrefLanguage?.Trim();
        _logger.LogInformation("🗣️ User Language is : " + pref);
        
        if (string.IsNullOrWhiteSpace(pref))
        {
            return;
        }
        
        var normalizedLanguageCode = NormalizeSupportedLanguageCode(pref);

        try
        {
            T.SetCulture(new CultureInfo(normalizedLanguageCode));

        }
        catch (CultureNotFoundException)
        {
            _logger.LogWarning("⚠️ Unsupported language code '{LanguageCode}', fallback to english.", pref);
            T.SetCulture(new CultureInfo("en"));
        }
    }
    
    private static string NormalizeSupportedLanguageCode(string languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
        {
            return "en";
        }

        var normalized = languageCode.Trim().ToLowerInvariant();

        if (normalized.StartsWith("de"))
        {
            return "de";
        }

        if (normalized.StartsWith("it"))
        {
            return "it";
        }

        if (normalized.StartsWith("fr"))
        {
            return "fr";
        }

        if (normalized.StartsWith("en"))
        {
            return "en";
        }

        return normalized switch
        {
            "german" or "deutsch" => "de",
            "italian" or "italiano" => "it",
            "french" or "francais" or "français" => "fr",
            _ => "en"
        };
    }
    
    private async Task SaveCurrentUserPreferences()
    {
        if (UserClientContext.CurrentUser is null)
        {
            return;
        }
        
        var rawUser = UserClientContext.CurrentUser.ToRaw();
        await Api.SaveAsync(rawUser);
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

        UserClientContext.CurrentUser.PrefLanguage = NormalizeSupportedLanguageCode(languageCode);
        ApplyUserLanguagePreference();
        await SaveCurrentUserPreferences();
        await InvokeAsync(StateHasChanged);
    }
}