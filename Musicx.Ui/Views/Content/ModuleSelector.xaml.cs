using System.Windows;
using log4net;
using Microsoft.Win32;
using Musicx.Core.Logging;
using Musicx.Infrastructure.Listeners;
using Musicx.Infrastructure.Services.LocalLibrary;

namespace Musicx.Ui.Views.Content;

public partial class ModuleSelector
{
    private static readonly ILogger Logger;

    class ProgressListener : IProgressListener
    {
        public void UpdateProgress(int progress, int total)
        {

        }
    }
    
    public ModuleSelector(ILoggerFactory loggerFactory)
    {
        InitializeComponent();
        Logger = loggerFactory.CreateLogger(typeof(ModuleSelector));
    }

    private async void LocalLibrary_Click(object sender, RoutedEventArgs e)
    {
        var fileBrowse = new OpenFolderDialog()
        {
            Title = "Select Audio Folder",
            Multiselect = true
        };

        if (true == fileBrowse.ShowDialog())
        {
            await ImportLocalSongs.Execute(fileBrowse.FolderNames.ToList(), [".mp3"], new ProgressListener());
        }
    }
}