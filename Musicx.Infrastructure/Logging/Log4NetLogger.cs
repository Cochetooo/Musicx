using System.Reflection;
using log4net;
using log4net.Config;
using Musicx.Core.Logging;

namespace Musicx.Infrastructure.Logging;

public class Log4NetLogger<T> : ILogger<T>
{
    private readonly ILog _logger;

    public Log4NetLogger()
    {
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly()!);
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        
        _logger = LogManager.GetLogger(typeof(T));
    }
    
    public void Debug(string message) => _logger.Debug(message);
    public void Info(string message) => _logger.Info(message);
    public void Warn(string message) => _logger.Warn(message);
    public void Error(string message) => _logger.Error(message);
    public void Fatal(string message) => _logger.Fatal(message);
    public void Fatal(string message, Exception exception) => _logger.Fatal(message, exception);
}

public class Log4NetLoggerFactory : ILoggerFactory
{
    public ILogger<T> CreateLogger<T>() => new Log4NetLogger<T>();
}