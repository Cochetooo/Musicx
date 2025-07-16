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

public sealed class MainTitleBarViewModel : ViewModelBase
{
    private readonly IContentViewProvider _contentViewProvider;

    public MainTitleBarViewModel(
        IWindowProvider windowProvider,
        IContentViewProvider contentViewProvider)
    {
        _contentViewProvider = contentViewProvider;
        
        // Menu Commands
        OpenLibraryManageCommand = ReactiveCommand.Create(OpenLibraryManage);
        
        // Window Commands
        MinimizeCommand = ReactiveCommand.Create(windowProvider.Minimize);
        ToggleMaximizeCommand = ReactiveCommand.Create(windowProvider.ToggleMaximize);
        CloseCommand = ReactiveCommand.Create(windowProvider.Close);
        StartDragCommand = ReactiveCommand.Create<PointerPressedEventArgs>(windowProvider.StartDrag);
    }
    
    #region Menu Commands
    
    public ICommand OpenLibraryManageCommand { get; }

    private void OpenLibraryManage()
    {
        _contentViewProvider.SwitchTo(App.Services.GetRequiredService<LibraryManageViewModel>());
    }
    
    #endregion
    
    #region Window Commands
    
    public ICommand MinimizeCommand { get; }
    public ICommand ToggleMaximizeCommand { get; }
    public ICommand CloseCommand { get; }
    public ICommand StartDragCommand { get; }
    
    #endregion
}