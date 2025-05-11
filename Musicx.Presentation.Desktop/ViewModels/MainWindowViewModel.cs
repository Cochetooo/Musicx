using System;
using Avalonia.Controls;
using Musicx.Presentation.Desktop.Services;
using ReactiveUI;

namespace Musicx.Presentation.Desktop.ViewModels;

public partial class MainWindowViewModel(IWindowProvider windowProvider) : ReactiveObject
{
    private ReactiveObject? _currentViewModel;
    
    public ReactiveObject? CurrentViewModel
    {
        get => _currentViewModel;
        set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    }
    
    public Window Window => windowProvider.GetMainWindow();
}