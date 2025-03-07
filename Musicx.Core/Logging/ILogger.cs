namespace Musicx.Core.Logging;

public interface ILogger
{
    void Debug(string message);
    void Info(string message);
    void Warn(string message);
    void Error(string message);
    void Fatal(string message);
    void Fatal(string message, Exception exception);
}

public interface ILoggerFactory
{
    ILogger CreateLogger(Type type);
}