namespace Musicx.Application.Common.Interfaces.Common;

/// <summary>
/// Base behavior for a progressing task listener.
/// </summary>
/// <since>0.2.0</since>
public interface IProgressListener
{
    /// <summary>
    /// Actual progress count.
    /// </summary>
    /// <since>0.6.0</since>
    public int Progress { get; set; }
    
    /// <summary>
    /// Total element count.
    /// </summary>
    /// <since>0.6.0</since>
    public int Total { get; set; }
    
    /// <summary>
    /// Additional parameters for callbacks.
    /// </summary>
    /// <since>0.6.0</since>
    public object? Parameters { get; set; }

    /// <summary>
    /// Update the progress count.
    /// </summary>
    /// <param name="progress">Actual progress count</param>
    /// <param name="total">Total element count</param>
    /// <param name="parameters">Additional parameters for callbacks</param>
    /// <since>0.6.0</since>
    public void UpdateProgress(int progress, int total, object? parameters);
}