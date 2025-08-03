namespace Musicx.Infrastructure.API.Persistence.Helpers;

public enum SqlConflictAction
{
    /// <summary>
    /// If the query has a conflict, do nothing.<br/>
    /// Equivalent to <c>ON CONFLICT DO NOTHING</c>
    /// </summary>
    Nothing,
    
    /// <summary>
    /// If the query has a conflict, throw an error.<br/>
    /// Equivalent to no command.
    /// </summary>
    Throw,
    
    /// <summary>
    /// If the query has a conflict, update instead of insert.<br/>
    /// Equivalent to <c>ON CONFLICT DO UPDATE</c>
    /// </summary>
    Update,
}