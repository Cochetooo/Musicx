using Microsoft.JSInterop;

namespace Musicx.Presentation.Web.Client.Providers;

public sealed class StyledJsConsoleLoggerProvider(IJSRuntime jsRuntime) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new StyledJsConsoleLogger(categoryName, jsRuntime);
    }
    
    public void Dispose() {}
}

public sealed class StyledJsConsoleLogger(string categoryName, IJSRuntime jsRuntime) : ILogger
{
    public IDisposable BeginScope<TState>(TState state) => null!;
    
    public bool IsEnabled(LogLevel logLevel) => true;

    public async void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (categoryName.StartsWith("System.Net"))
        {
            return;
        }
        
        string message = formatter(state, exception);
        string level = logLevel.ToString().ToUpper();
        string date = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss");

        string logLine = $"[{date}] {level,-5} <{categoryName}> - {message}";
        string css = logLevel switch
        {
            LogLevel.Trace => "color: lightseagreen; font-style: italic;",
            LogLevel.Debug => "color: lightgrey; font-style: italic;",
            LogLevel.Information => "color: lightgreen;",
            LogLevel.Warning => "color: gold;",
            LogLevel.Error => "color: indianred;",
            LogLevel.Critical => "color: white; background: indianred; font-weight: bold;",
            _ => "color: black;",
        };
        
        await jsRuntime.InvokeVoidAsync($"console.log", $"%c{logLine}", css);
    }
}