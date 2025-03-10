using System.Windows.Controls;
using System.Windows.Input;
using Musicx.Core.Models;
using Musicx.Ui.Pages.LocalLibrary.ViewModels;

namespace Musicx.Ui.Pages.LocalLibrary.Views;

public partial class LocalLibraryView : UserControl
{
    public LocalLibraryView()
    {
        InitializeComponent();
    }
    
    private void SongDataGrid_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SongDataGrid.SelectedItem is Song selectedSong)
        {
            if (DataContext is LocalLibraryViewModel viewModel)
            {
                viewModel.SelectedSong = selectedSong;
            }
        }
    }
}