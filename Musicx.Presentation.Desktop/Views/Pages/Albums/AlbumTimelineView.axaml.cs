using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Presentation.Desktop.Views.Pages.Albums;

public partial class AlbumTimelineView : UserControl
{
    public static readonly StyledProperty<IEnumerable<OutAlbum>> AlbumsProperty =
        AvaloniaProperty.Register<AlbumTimelineView, IEnumerable<OutAlbum>>(nameof(Albums), []);

    public IEnumerable<OutAlbum> Albums
    {
        get => GetValue(AlbumsProperty);
        set => SetValue(AlbumsProperty, value);
    }

    public AlbumTimelineView() => InitializeComponent();
}