namespace Musicx.Application.Common.Interfaces.Common;

/// <summary>
/// Base behavior for the logger.
/// </summary>
/// <typeparam name="T">The class that implements the logger</typeparam>
/// <since>0.6.0</since>
public interface ILogger<T>
{
    /// <summary>
    /// Logs a message with the DB level.
    /// </summary>
    /// <param name="message">The message object to log</param>
    /// <since>0.6.1</since>
    void Db(object message);
    
    /// <summary>
    /// Logs a message with the DEBUG level.
    /// </summary>
    /// <param name="message">The message object to log</param>
    /// <since>0.6.0</since>
    void Debug(object message);
    
    /// <summary>
    /// Logs a message with the INFO level.
    /// </summary>
    /// <param name="message">The message object to log</param>
    /// <since>0.6.0</since>
    void Info(object message);
    
    /// <summary>
    /// Logs a message with the WARN level.
    /// </summary>
    /// <param name="message">The message object to log</param>
    /// <since>0.6.0</since>
    void Warn(object message);
    
    /// <summary>
    /// Logs a message with the ERROR level.
    /// </summary>
    /// <param name="message">The message object to log</param>
    /// <since>0.6.0</since>
    void Error(object message);
    
    /// <summary>
    /// Logs a message with the FATAL level.
    /// </summary>
    /// <param name="message">The message object to log</param>
    /// <since>0.6.0</since>
    void Fatal(object message);
    
    /// <summary>
    /// Logs a message with the FATAL level including the stack trace of the <see cref="Exception"/>
    /// passed as parameter.
    /// </summary>
    /// <param name="message">The message object to log</param>
    /// <param name="exception">The exception to log, including its stack trace.</param>
    /// <since>0.6.0</since>
    void Fatal(object message, Exception exception);
}

/// <summary>
/// Base behavior for the factory that will instantiate loggers.
/// </summary>
/// <since>0.6.0</since>
public interface ILoggerFactory
{
    /// <summary>
    /// Instantiate a new logger.
    /// </summary>
    /// <typeparam name="T">The class that implements the logger.</typeparam>
    /// <returns>A logger object</returns>
    /// <since>0.6.0</since>
    ILogger<T> CreateLogger<T>();
}