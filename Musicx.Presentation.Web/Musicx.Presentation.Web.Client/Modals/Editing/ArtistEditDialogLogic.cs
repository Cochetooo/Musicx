using Musicx.Contracts.Dto.Enums;
using Musicx.Contracts.Dto.Requests;

namespace Musicx.Presentation.Web.Client.Modals.Editing;

public sealed class ArtistEditDialogLogic : BaseEditDialogLogic<InArtist>
{
    public override async Task LoadAsync()
    {
        Entity.Discriminator = ArtistDiscriminator.Artist;
    }
}