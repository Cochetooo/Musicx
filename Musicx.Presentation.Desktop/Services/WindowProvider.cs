using System;
using Avalonia.Controls;

namespace Musicx.Presentation.Desktop.Services;

public interface IWindowProvider
{
    Window GetMainWindow();
}

public sealed class WindowProvider : IWindowProvider
{
    private readonly Lazy<Window> _window;

    public WindowProvider(Func<Window> windowFactory)
    {
        _window = new Lazy<Window>(windowFactory);
    }
    
    public Window GetMainWindow() => _window.Value;
}