using System;
using Avalonia.Controls;
using Avalonia.Input;

namespace Musicx.Presentation.Desktop.Services;

public interface IWindowProvider
{
    void Close();
    void Minimize();
    void StartDrag(PointerPressedEventArgs args);
    void ToggleMaximize();
}

public sealed class WindowProvider : IWindowProvider
{
    private Window? _window;
    
    public void SetWindow(Window window) => _window = window;
    private Window GetWindow() =>
        _window ?? throw new InvalidOperationException("Window is not set");

    public void Close() => GetWindow().Close();
    public void Minimize() => GetWindow().WindowState = WindowState.Minimized;
    public void StartDrag(PointerPressedEventArgs args) => GetWindow().BeginMoveDrag(args);
    public void ToggleMaximize() => GetWindow().WindowState = GetWindow().WindowState == WindowState.Maximized 
        ? WindowState.Normal 
        : WindowState.Maximized;
}