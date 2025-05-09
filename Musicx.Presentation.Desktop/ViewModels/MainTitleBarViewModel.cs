using System.Windows.Input;
using Avalonia.Controls;
using ReactiveUI;

namespace Musicx.Presentation.Desktop.ViewModels;

public sealed class MainTitleBarViewModel : ReactiveObject
{
    private readonly Window _window;

    public MainTitleBarViewModel(Window window)
    {
        _window = window;
        
        MinimizeCommand = ReactiveCommand.Create(() => _window.WindowState = WindowState.Minimized);

        ToggleMaximizeCommand = ReactiveCommand.Create(() =>
        {
            _window.WindowState = _window.WindowState == WindowState.Maximized 
                ? WindowState.Normal 
                : WindowState.Maximized;
        });
        
        CloseCommand = ReactiveCommand.Create(() => _window.Close());
    }
    
    public ICommand MinimizeCommand { get; }
    public ICommand ToggleMaximizeCommand { get; }
    public ICommand CloseCommand { get; }
}