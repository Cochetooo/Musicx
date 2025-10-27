using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Contracts.Dto.Requests;

namespace Musicx.Presentation.Web.Client.Modals.Editing;

/// <summary>
/// Base logic for each edit modals.
/// Bring dialog management, logging, snackbars and life cycle Save/Close
/// </summary>
public abstract class BaseEditDialogLogic<T> where T : BaseInputModel, new()
{
    [Inject] protected ISnackbar Snackbar { get; set; } = null!;
    [Inject] protected ILoggerFactory LoggerFactory { get; set; } = null!;
    protected ILogger _logger = null!;
    
    public bool IsSaving { get; private set; }
    public bool IsLoading { get; private set; }
    public T Entity { get; private set; } = new();

    protected virtual void Initialize()
    {
        _logger = LoggerFactory.CreateLogger(GetType());
    }
    
    public virtual Task LoadAsync() => Task.CompletedTask;

    public virtual async Task<bool> SaveAsync(Func<Task<bool>> persistAction)
    {
        try
        {
            IsSaving = true;
            var success = await persistAction();

            if (success)
            {
                Snackbar.Add("✅ Changes saved successfully!", Severity.Success);
            }
            else
            {
                Snackbar.Add("❌ Could not save changes!", Severity.Error);
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error while saving {EntityName}", typeof(T).Name);
            Snackbar.Add($"Unexpected error while saving: {ex.Message}", Severity.Error);

            return false;
        }
        finally
        {
            IsSaving = false;
        }
    }
    
    protected void Warn(string message)
        => Snackbar.Add(message, Severity.Warning, config => config.RequireInteraction = true);
    
    protected void Error(string message)
        => Snackbar.Add(message, Severity.Warning, config => config.RequireInteraction = true);
    
    protected void Info(string message)
        => Snackbar.Add(message, Severity.Info);
}