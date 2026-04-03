using System;
using Microsoft.Extensions.Logging;

namespace Musicx.Presentation.Desktop.Providers;

public sealed class StyledConsoleLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new StyledConsoleLogger(categoryName);

    public void Dispose()
    {
    }

    private sealed class StyledConsoleLogger(string categoryName) : ILogger
    {
        public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = logLevel switch
            {
                LogLevel.Trace => ConsoleColor.DarkGray,
                LogLevel.Debug => ConsoleColor.Gray,
                LogLevel.Information => ConsoleColor.Cyan,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Critical => ConsoleColor.Magenta,
                _ => ConsoleColor.White
            };

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{logLevel}] {categoryName}: {formatter(state, exception)}");
            if (exception is not null)
            {
                Console.WriteLine(exception);
            }

            Console.ForegroundColor = previousColor;
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();
            public void Dispose() { }
        }
    }
}