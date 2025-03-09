using Microsoft.Extensions.DependencyInjection;
using Musicx.Ui.ViewModels.Content;
using Musicx.Ui.ViewModels.LocalLibrary;

namespace Musicx.Ui.ViewModels;

public class ViewModelLocator
{
    public static IServiceProvider ServiceProvider { get; set; } = null!;
    
    public MainWindowViewModel MainWindowViewModel => ServiceProvider.GetRequiredService<MainWindowViewModel>();
    
    public ModuleSelectorViewModel ModuleSelectorVm => ServiceProvider.GetRequiredService<ModuleSelectorViewModel>();
    
    public SongListViewModel SongListVm => ServiceProvider.GetRequiredService<SongListViewModel>();
}