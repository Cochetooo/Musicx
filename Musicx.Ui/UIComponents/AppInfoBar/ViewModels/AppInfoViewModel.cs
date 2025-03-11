using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Musicx.Infrastructure.Listeners;
using Musicx.Infrastructure.Managers;

namespace Musicx.Ui.UIComponents.AppInfoBar.ViewModels;

public partial class AppInfoViewModel : ObservableObject
{
    [ObservableProperty] private string _rightContentText;
    [ObservableProperty] private string _leftContentText;
    [ObservableProperty] private double _progressValue;
    [ObservableProperty] private bool _isProgressVisible;
    
    private readonly ProgressListener _progressListener;

    public AppInfoViewModel(ISongManager songManager, IProgressListener progressListener)
    {
        _progressListener = (ProgressListener) progressListener;
        _progressListener.PropertyChanged += OnProgressChanged;
        
        LeftContentText = "";
        RightContentText = $"{songManager.GetCount().Result} files, --:--:--, 0 MB";
        IsProgressVisible = false;
    }

    private void OnProgressChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ProgressListener.Progress) || 
            e.PropertyName == nameof(ProgressListener.Total) || 
            e.PropertyName == nameof(ProgressListener.Parameters))
        {
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (_progressListener.Total > 0)
        {
            ProgressValue = (double) _progressListener.Progress / _progressListener.Total;
            LeftContentText = $"Loading {_progressListener.Parameters ?? "..."} ({ProgressValue:P0})";
            IsProgressVisible = ProgressValue < 1;
        }
        else
        {
            LeftContentText = string.Empty;
            ProgressValue = 0;
            IsProgressVisible = false;
        }
    }
}