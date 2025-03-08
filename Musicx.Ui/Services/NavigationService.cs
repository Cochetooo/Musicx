using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Musicx.Ui.Services;

public class NavigationService : INotifyPropertyChanged
{
    private object _currentView;

    public object CurrentView
    {
        get => _currentView;
        set
        {
            _currentView = value;
            OnPropertyChanged();
        }
    }

    public void NavigateTo(object view)
    {
        CurrentView = view;
    }
     
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}