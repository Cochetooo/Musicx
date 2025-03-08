using Microsoft.Extensions.DependencyInjection;
using Musicx.Ui.ViewModels.Content;
using Musicx.Ui.ViewModels.LocalLibrary;

namespace Musicx.Ui.ViewModels;

public class ViewModelLocator
{
    public static IServiceProvider ServiceProvider { get; set; } = null!;
    
    public MainViewModel MainViewModel => ServiceProvider.GetRequiredService<MainViewModel>();
    
    public ModuleSelectorViewModel ModuleSelectorVm => ServiceProvider.GetRequiredService<ModuleSelectorViewModel>();
    
    public SongListViewModel SongListVm => ServiceProvider.GetRequiredService<SongListViewModel>();
}