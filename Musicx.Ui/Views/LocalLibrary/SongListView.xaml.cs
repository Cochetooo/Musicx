using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using Musicx.Core.Models;
using Musicx.Data.Entities;
using Musicx.Infrastructure.Managers;
using Musicx.Ui.ViewModels.LocalLibrary;

namespace Musicx.Ui.Views.LocalLibrary;

public partial class SongListView : UserControl
{
    public SongListView()
    {
        InitializeComponent();
    }
    
    private void SongDataGrid_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SongDataGrid.SelectedItem is Song selectedSong)
        {
            var viewModel = DataContext as SongListViewModel;
            viewModel?.PlaySongCommand.Execute(selectedSong);
        }
    }
}