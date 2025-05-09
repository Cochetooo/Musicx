using System;
using Avalonia.Controls;
using Musicx.Presentation.Desktop.ViewModels;

namespace Musicx.Presentation.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();

        MainTitleBar.DataContext = new MainTitleBarViewModel(this);
    }
}