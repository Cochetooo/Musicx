using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Musicx.Presentation.Desktop.ViewModels.Body.Content;

namespace Musicx.Presentation.Desktop.Views.Body.Content;

public partial class LibraryManageView : UserControl
{
    public LibraryManageView()
    {
        InitializeComponent();

        DataContext = App.Services.GetRequiredService<LibraryManageViewModel>();
    }
}