using System;
using System.Reflection;
using Avalonia.Controls;
using Musicx.Presentation.Desktop.Services;
using Musicx.Presentation.Desktop.ViewModels.Body.Content;
using ReactiveUI;

namespace Musicx.Presentation.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    #region View models
    
    public MainTitleBarViewModel MainTitleBarViewModel { get; }
    public BottomAudioPlayerViewModel BottomAudioPlayerViewModel { get; }
    public SidebarLeftViewModel SidebarLeftViewModel { get; }
    public SidebarRightViewModel SidebarRightViewModel { get; }
    
    #endregion
    
    #region Reactive Fields
    
    private readonly IContentViewProvider _contentViewProvider;
    
    public ReactiveObject? CurrentViewModel => _contentViewProvider.Content;
    
    #endregion
    
    #region Constructors

    public MainWindowViewModel(
        MainTitleBarViewModel mainTitleBarViewModel,
        BottomAudioPlayerViewModel bottomAudioPlayerViewModel,
        SidebarLeftViewModel sidebarLeftViewModel,
        SidebarRightViewModel sidebarRightViewModel,
        IContentViewProvider contentViewProvider)
    {
        MainTitleBarViewModel = mainTitleBarViewModel;
        BottomAudioPlayerViewModel = bottomAudioPlayerViewModel;
        SidebarLeftViewModel = sidebarLeftViewModel;
        SidebarRightViewModel = sidebarRightViewModel;
        _contentViewProvider = contentViewProvider;

        _contentViewProvider.WhenAnyValue(x => x.Content)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(CurrentViewModel)));
    }
    
    #endregion
}