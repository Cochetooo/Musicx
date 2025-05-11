using System;
using System.Reactive;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Input;
using Microsoft.Extensions.DependencyInjection;
using Musicx.Presentation.Desktop.Services;
using Musicx.Presentation.Desktop.ViewModels.Body.Content;
using Musicx.Presentation.Desktop.Views;
using ReactiveUI;

namespace Musicx.Presentation.Desktop.ViewModels;

public sealed class MainTitleBarViewModel : ReactiveObject
{
    private readonly Window _window;
    private readonly MainWindowViewModel _mainWindow;

    public MainTitleBarViewModel(MainWindowViewModel mainWindow)
    {
        _mainWindow = mainWindow;
        _window = mainWindow.Window;
        
        // Menu Commands
        OpenLibraryManageCommand = ReactiveCommand.Create(OpenLibraryManage);
        
        // Window Commands
        MinimizeCommand = ReactiveCommand.Create(Minimize);
        ToggleMaximizeCommand = ReactiveCommand.Create(ToggleMaximize);
        CloseCommand = ReactiveCommand.Create(Close);
        StartDragCommand = ReactiveCommand.Create<PointerPressedEventArgs>(StartDrag);
    }
    
    #region Menu Commands
    
    public ICommand OpenLibraryManageCommand { get; }

    private void OpenLibraryManage()
    {
        _mainWindow.CurrentViewModel = App.Services.GetRequiredService<LibraryManageViewModel>();
    }
    
    #endregion
    
    #region Window Commands
    
    public ICommand MinimizeCommand { get; }
    
    private void Minimize() => _window.WindowState = WindowState.Minimized;
    
    public ICommand ToggleMaximizeCommand { get; }
    
    private void ToggleMaximize() => _window.WindowState = _window.WindowState == WindowState.Maximized 
        ? WindowState.Normal 
        : WindowState.Maximized;
    
    public ICommand CloseCommand { get; }
    
    private void Close() => _window.Close();
    public ICommand StartDragCommand { get; }
    
    private void StartDrag(PointerPressedEventArgs e) => _window.BeginMoveDrag(e);
    
    #endregion
}