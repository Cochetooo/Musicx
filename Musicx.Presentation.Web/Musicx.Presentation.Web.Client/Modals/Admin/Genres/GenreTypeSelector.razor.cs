using Microsoft.AspNetCore.Components;
using Musicx.Contracts.Dto.Requests.Genre;
using Musicx.Contracts.Enums;

namespace Musicx.Presentation.Web.Client.Modals.Admin.Genres;

public partial class GenreTypeSelector
{
    [Parameter] public InGenre Node { get; set; } = null!;
    [Parameter] public EventCallback NodeChanged { get; set; }

    public void Select(GenreType type)
    {
        if (type == Node.Type)
        {
            return;
        }
        
        Node.Type = type;
        NodeChanged.InvokeAsync();
    }
}