using Microsoft.Extensions.DependencyInjection;
using Musicx.Ui.Main.ViewModels;
using Musicx.Ui.Pages.LocalLibrary.ViewModels;
using Musicx.Ui.Pages.PageSelector.ViewModels;
using Musicx.Ui.UIComponents.AppInfoBar.ViewModels;
using Musicx.Ui.UIComponents.AudioPlayer.ViewModels;

namespace Musicx.Ui.Core;

public class ViewModelLocator
{
    public static IServiceProvider ServiceProvider { get; set; } = null!;

    public MainWindowViewModel MainWindowViewModel => ServiceProvider.GetRequiredService<MainWindowViewModel>();

    public PageSelectorViewModel PageSelectorVm => ServiceProvider.GetRequiredService<PageSelectorViewModel>();

    public LocalLibraryViewModel LocalLibraryVm => ServiceProvider.GetRequiredService<LocalLibraryViewModel>();

    // Ui Components
    public AppInfoViewModel AppInfoVm => ServiceProvider.GetRequiredService<AppInfoViewModel>();
    public AudioPlayerViewModel AudioPlayerVm => ServiceProvider.GetRequiredService<AudioPlayerViewModel>();
}