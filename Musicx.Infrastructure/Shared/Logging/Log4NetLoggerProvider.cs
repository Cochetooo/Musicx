using System.Reflection;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.Extensions.Logging;

namespace Musicx.Infrastructure.Shared.Logging;

internal sealed class Log4NetLoggerProvider : ILoggerProvider
{
    private readonly ILoggerRepository _repository;

    public Log4NetLoggerProvider(string configFile = "log4net.config")
    {
        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        _repository = LogManager.GetRepository(assembly);
        XmlConfigurator.Configure(_repository, new FileInfo(configFile));
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new Log4NetLogger(categoryName);
    }

    public void Dispose() {}
}