using CommunityToolkit.Mvvm.ComponentModel;

namespace Musicx.Infrastructure.Listeners;

public interface IProgressListener
{
    public int Progress { get; set; }
    public int Total { get; set; }
    public object? Parameters { get; set; }
    public void UpdateProgress(int progress, int total, object? parameters);
}

public partial class ProgressListener : ObservableObject, IProgressListener
{
    [ObservableProperty] private int _progress;
    [ObservableProperty] private int _total;
    [ObservableProperty] private object? _parameters;
    
    public void UpdateProgress(int progress, int total, object? parameters)
    {
        Progress = progress;
        Total = total;
        Parameters = parameters;
    }
}