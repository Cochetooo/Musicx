using System.Reflection;
using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Repository;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Common;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Musicx.Infrastructure.Shared.Logging;

public sealed class Log4NetLogger(string categoryName) : ILogger
{
    private readonly ILog _logger = LogManager.GetLogger(categoryName);

    private static readonly Level DbLevel = new Level(10_000, "DB");

    public IDisposable BeginScope<TState>(TState state) => null!;
    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter)
    {
        var message = formatter(state, exception);

        switch (logLevel)
        {
            case LogLevel.Trace:
            case LogLevel.Debug:
                _logger.Debug(message);
                break;
            case LogLevel.Information:
                _logger.Info(message);
                break;
            case LogLevel.Warning:
                _logger.Warn(message);
                break;
            case LogLevel.Error:
                _logger.Error(message);
                break;
            case LogLevel.Critical:
                _logger.Fatal(message);
                break;
            default:
                _logger.Info(message);
                break;
        }
    }

    public void Db(string message)
    {
        _logger.Logger.Log(typeof(Log4NetLogger), DbLevel, message, null);
    }
}