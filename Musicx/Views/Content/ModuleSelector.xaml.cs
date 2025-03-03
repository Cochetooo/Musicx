using System.Windows;
using log4net;
using Microsoft.Win32;
using Musicx.Services.Audio;
using MusicxApi.Models;
using NAudio.Wave;

namespace Musicx.Views.Content;

public partial class ModuleSelector
{
    private static readonly ILog Logger = LogManager.GetLogger(typeof(ModuleSelector));
    
    public ModuleSelector()
    {
        InitializeComponent();
    }

    private void TestReadAudioFile(object sender, RoutedEventArgs e)
    {
        var fileBrowse = new OpenFileDialog
        {
            Title = "Select Audio File",
            Filter = "MP3 Files (*.mp3)|*.mp3|All Files (*.*)|*.*"
        };

        Song song;

        if (true == fileBrowse.ShowDialog())
        {
            ReadAudioFile.Execute(fileBrowse.FileName, out song);
        }
    }
}