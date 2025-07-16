using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Microsoft.Extensions.DependencyInjection;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Presentation.Desktop.ViewModels;

namespace Musicx.Presentation.Desktop;

public class ViewLocator(ILoggerFactory loggerFactory) : IDataTemplate
{
    private readonly ILogger<ViewLocator> _logger = loggerFactory.CreateLogger<ViewLocator>();
    
    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        
        _logger.Debug($"📒 View model {param.GetType().FullName!} is searching for view {name}");
        
        var type = AppDomain.CurrentDomain
            .GetAssemblies()
            .Select(a => a.GetType(name))
            .FirstOrDefault(t => t != null);

        if (type != null)
        {
            _logger.Info("✅ Found view : " + type.FullName);
            return (Control)Activator.CreateInstance(type)!;
        }

        _logger.Warn("⚠️ View has not been found: " + name);
        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data) => data is ViewModelBase;
}