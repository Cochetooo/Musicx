using System.Reflection;
using log4net;
using log4net.Config;
using log4net.Core;
using Musicx.Application.Common.Interfaces.Common;

namespace Musicx.Infrastructure.Common.Logging;

public class Log4NetLogger<T> : ILogger<T>
{
    private readonly ILog _logger = LogManager.GetLogger(typeof(T));
    private static readonly Level dbLevel = new(10_000, "DB");

    public void Db(object message)
    {
        _logger.Logger.Log(typeof(T), dbLevel, message, null);
    }
    public void Debug(object message) => _logger.Debug(message);
    public void Info(object message) => _logger.Info(message);
    public void Warn(object message) => _logger.Warn(message);
    public void Error(object message) => _logger.Error(message);
    public void Fatal(object message) => _logger.Fatal(message);
    public void Fatal(object message, Exception exception) => _logger.Fatal(message, exception);
}

public class Log4NetLoggerFactory : ILoggerFactory
{
    public Log4NetLoggerFactory()
    {
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly()!);
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
    }
    
    public ILogger<T> CreateLogger<T>() => new Log4NetLogger<T>();
}