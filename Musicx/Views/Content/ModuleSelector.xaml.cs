using System.Windows;
using log4net;
using Microsoft.Win32;
using Musicx.Listeners;
using Musicx.Services.Audio;
using Musicx.Services.LocalLibrary;
using MusicxApi.Models;
using NAudio.Wave;

namespace Musicx.Views.Content;

public partial class ModuleSelector
{
    private static readonly ILog Logger = LogManager.GetLogger(typeof(ModuleSelector));

    class ProgressListener : IProgressListener
    {
        public void UpdateProgress(int progress, int total)
        {

        }
    }
    
    public ModuleSelector()
    {
        InitializeComponent();
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