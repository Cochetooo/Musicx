using System.ComponentModel;
using Musicx.Ui.ViewModels.Content;
using Musicx.Ui.ViewModels.LocalLibrary;

namespace Musicx.Ui.ViewModels;

public interface IViewModelNavigator
{
    object CurrentViewModel { get; }
    void ChangeViewModel(object viewModel);
}

public class ViewModelNavigator : IViewModelNavigator, INotifyPropertyChanged
{
    private readonly Func<ModuleSelectorViewModel> _moduleSelectorVmFactory;
    private readonly Func<SongListViewModel> _songListVmFactory;

    private object _currentViewModel;
    public object CurrentViewModel
    {
        get => _currentViewModel;
        private set
        {
            _currentViewModel = value;
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }

    public ViewModelNavigator(
        Func<ModuleSelectorViewModel> moduleSelectorVmFactory,
        Func<SongListViewModel> songListVmFactory)
    {
        _moduleSelectorVmFactory = moduleSelectorVmFactory;
        _songListVmFactory = songListVmFactory;

        // Initialisation avec ModuleSelectorViewModel
        CurrentViewModel = _moduleSelectorVmFactory();
    }

    public void ChangeViewModel(object viewModel)
    {
        CurrentViewModel = viewModel;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) 
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
