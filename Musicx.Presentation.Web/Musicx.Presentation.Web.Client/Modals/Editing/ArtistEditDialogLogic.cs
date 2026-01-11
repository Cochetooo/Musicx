using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Artist;
using Musicx.Contracts.Enums;

namespace Musicx.Presentation.Web.Client.Modals.Editing;

public sealed class ArtistEditDialogLogic : BaseEditDialogLogic<InArtist>
{
    public override async Task LoadAsync()
    {
        Entity.Discriminator = ArtistDiscriminator.Artist;
    }
}