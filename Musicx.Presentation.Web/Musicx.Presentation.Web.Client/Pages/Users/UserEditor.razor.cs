using ISO3166;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.User;
using Musicx.Contracts.Enums;
using Musicx.Infrastructure.API.Persistence.Mappers;

namespace Musicx.Presentation.Web.Client.Pages.Users;

public partial class UserEditor : IAsyncDisposable
{
    private OutUser? _user;
    private string? _newPassword;
    private string? _confirmPassword;
    private string _activeSettingsCategory = "display";

    private IJSObjectReference? _userEditorModule;
    private DotNetObjectReference<UserEditor>? _dotNetRef;
    
    private readonly List<string> _countries = [];

    private readonly List<string> SettingsCategories =
    [
        "display",
        "ratings",
        "language",
        "connections"
    ];
    
    private readonly List<(RatingMode, string)> RatingModes =
    [
        new(RatingMode.OutOfFive, Icons.Material.Filled.Star),
        new(RatingMode.OutOfTen, Icons.Material.Filled.StarBorder),
        new(RatingMode.OutOfTwenty, Icons.Material.Filled.Grade),
        new(RatingMode.OutOfFifty, Icons.Material.Filled.Grade),
        new(RatingMode.OutOfThousand, Icons.Material.Filled.Grade),
        new(RatingMode.Percentage, Icons.Material.Filled.Percent),
        new(RatingMode.TierList, Icons.Material.Filled.ViewModule),
        new(RatingMode.TierListDetailed, Icons.Material.Filled.ViewComfy),
        new(RatingMode.TextualShort, Icons.Material.Filled.TextFields),
        new(RatingMode.TextualDetailed, Icons.Material.Filled.Description),
        new(RatingMode.RatingStars, Icons.Material.Filled.StarRate),
    ];
    
    private readonly List<(string Code, string Label, string CountryName)> Languages =
    [
        ("de", "Deutsch", "Germany"),
        ("en", "English", "United States"),
        ("fr", "Français", "France"),
        ("it", "Italiano", "Italy")
    ];

    protected override void OnInitialized()
    {
        _user = UserClientContext.CurrentUser;
        _countries.AddRange(Country.List
            .Select(c => c.Name)
            .Distinct()
            .OrderBy(name => name));
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        _dotNetRef = DotNetObjectReference.Create(this);
        _userEditorModule = await JS.InvokeAsync<IJSObjectReference>("import", "/Js/userEditor.js");
        await _userEditorModule.InvokeVoidAsync("setupSettingsSpy", "#settings-scroll-container", _dotNetRef);
    }

    private async Task OnAvatarSelected(InputFileChangeEventArgs e)
    {
        if (_user is null)
        {
            Snackbar.Add(T["Web.UserEditor.Errors.NoUser"], Severity.Warning);
            return;
        }
        
        var file = e.File;
        await using var stream = file.OpenReadStream(5_000_000);
        _user.PictureUrl = await UcSaveAvatar.ExecuteAsync(stream, file.ContentType);

        await InvokeAsync(StateHasChanged);
    }

    private void SetRatingMode(RatingMode mode)
    {
        if (_user is null)
        {
            Snackbar.Add(T["Web.UserEditor.Errors.NoUser"], Severity.Warning);
            return;
        }
        
        _user.PrefRatingMode = mode;
    }

    private async Task SavePassword()
    {
        if (_user is null)
        {
            Snackbar.Add(T["Web.UserEditor.Errors.NoUser"], Severity.Warning);
            return;
        }
        
        if (string.IsNullOrWhiteSpace(_newPassword) || _newPassword != _confirmPassword)
        {
            Snackbar.Add(T["Web.UserEditor.Errors.PasswordMismatch"], Severity.Error);
            return;
        }
        
        await UcSavePassword.ExecuteAsync(_user.Id, _newPassword);
        _newPassword = null;
        _confirmPassword = null;
        Snackbar.Add(T["Web.UserEditor.PasswordSaved"], Severity.Success);
    }

    private async Task Save()
    {
        if (_user is null)
        {
            Snackbar.Add(T["Web.UserEditor.Errors.NoUser"], Severity.Warning);
            return;
        }

        await Api.SaveAsync(_user.ToRaw());
        Snackbar.Add(T["Web.UserEditor.UserSaved"], Severity.Success);
        await UserClientContext.RefreshAsync();
        
        Navigation.NavigateTo($"/User/{_user.Id}");
    }
    
    private string GetCategoryButtonClass(string categoryId)
        => categoryId == _activeSettingsCategory
            ? "settings-nav-button is-active"
            : "settings-nav-button";

    private string GetCategoryLabel(string categoryId)
        => T[$"Web.UserEditor.Category.{categoryId}"];

    private async Task ScrollToSettingsCategory(string categoryId)
    {
        _activeSettingsCategory = categoryId;

        if (_userEditorModule is not null)
        {
            await _userEditorModule.InvokeVoidAsync("scrollToSettingsSection", "#settings-scroll-container", categoryId);
        }
    }

    [JSInvokable]
    public async Task SetActiveSettingsCategory(string categoryId)
    {
        if (string.IsNullOrWhiteSpace(categoryId) || _activeSettingsCategory == categoryId)
        {
            return;
        }

        _activeSettingsCategory = categoryId;
        await InvokeAsync(StateHasChanged);
    }
    
    private async Task<IEnumerable<string>> SearchCountry(string? value, CancellationToken token)
    {
        await Task.Delay(5, token);

        if (string.IsNullOrWhiteSpace(value))
        {
            return _countries;
        }

        return _countries.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));
    }

    private async Task SignOut()
    {
        await Http.PostAsync("/api/auth/signout", null);
        await UserClientContext.RefreshAsync();
        Navigation.NavigateTo("/");
    }

    private async Task DeleteAccount()
    {
        if (_user is null)
        {
            return;
        }
        
        var result = await DialogService.ShowMessageBoxAsync(
            T["Web.Common.Warning"],
            T["Web.UserEditor.DeleteWarning"],
            yesText: T["Web.Common.Delete"],
            cancelText: T["Web.Common.Cancel"]);

        if (result is not true)
        {
            return;
        }

        await Api.DeleteAsync<InUser>(_user.Id);
        await Http.PostAsync("/api/auth/signout", null);
        await UserClientContext.RefreshAsync();
        Snackbar.Add(T["Web.UserEditor.AccountDeleted"], Severity.Success);
        
        Navigation.NavigateTo("/");
    }
    
    public async ValueTask DisposeAsync()
    {
        _dotNetRef?.Dispose();

        if (_userEditorModule is not null)
        {
            await _userEditorModule.DisposeAsync();
        }
    }
}