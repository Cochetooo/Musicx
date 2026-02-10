using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Enums;
using Musicx.Infrastructure.API.Persistence.Mappers;

namespace Musicx.Presentation.Web.Client.Pages.Users;

public partial class UserEditor
{
    private OutUser? _user;
    private string? _newPassword;
    private string? _confirmPassword;
    
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

    protected override void OnInitialized()
    {
        _user = UserClientContext.CurrentUser;
    }

    private async Task OnAvatarSelected(InputFileChangeEventArgs e)
    {
        if (_user is null)
        {
            Snackbar.Add("User is null, cannot upload avatar.", Severity.Warning);
            return;
        }
        
        var file = e.File;
        await using var stream = file.OpenReadStream(5_000_000);
        _user.PictureUrl = await UcSaveAvatar.ExecuteAsync(stream, file.ContentType);
    }
    
    private void SetRatingMode(RatingMode mode) => _user!.PrefRatingMode = mode;

    private async Task Save()
    {
        if (_user is null)
        {
            Snackbar.Add("User is null, cannot save data.", Severity.Warning);
            return;
        }

        var rawUser = _user.ToRaw();
        
        if (!string.IsNullOrWhiteSpace(_newPassword) && _newPassword == _confirmPassword)
        {
            rawUser.Password = _newPassword;
        }

        await UcSave.ExecuteAsync(rawUser);
        Snackbar.Add("User has been saved.", Severity.Success);
        
        Navigation.NavigateTo("/User/" + _user.Id);
    }
}