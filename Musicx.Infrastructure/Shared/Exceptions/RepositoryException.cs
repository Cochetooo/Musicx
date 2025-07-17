using Microsoft.Extensions.Logging;

namespace Musicx.Infrastructure.Shared.Exceptions;

public sealed class RepositoryException : Exception
{
    public RepositoryException(string message, Exception innerException, ILogger logger)
    : base(message, innerException)
    {
        logger.LogCritical(message + "\n" + innerException.Message + "\n" + innerException.StackTrace);
    }
}