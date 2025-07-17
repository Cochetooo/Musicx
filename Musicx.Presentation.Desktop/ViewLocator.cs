using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Presentation.Desktop.ViewModels;

namespace Musicx.Presentation.Desktop;

public class ViewLocator(ILoggerProvider loggerProvider) : IDataTemplate
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(ViewLocator));
    
    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        
        _logger.LogDebug($"📒 View model {param.GetType().FullName!} is searching for view {name}");
        
        var type = AppDomain.CurrentDomain
            .GetAssemblies()
            .Select(a => a.GetType(name))
            .FirstOrDefault(t => t != null);

        if (type != null)
        {
            _logger.LogInformation("✅ Found view : " + type.FullName);
            return (Control)Activator.CreateInstance(type)!;
        }

        _logger.LogWarning("⚠️ View has not been found: " + name);
        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data) => data is ViewModelBase;
}