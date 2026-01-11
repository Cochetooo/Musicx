using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Artist;

namespace Musicx.Presentation.Web.Client.Modals.Editing;

public partial class ArtistEditDialog
{
    private ArtistEditDialogLogic _artistLogic = new();

    protected override async Task OnInitializedAsync()
    {
        
    }

    private async Task<bool> SaveArtistAsync(InArtist artist)
    {
        var response = await UcSave.ExecuteAsync(artist);
        return response.IsSuccessStatusCode;
    }
}