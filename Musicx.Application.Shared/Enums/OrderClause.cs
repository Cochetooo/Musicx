namespace Musicx.Application.Shared.Enums;

public sealed record OrderClause(
    string Field,
    OrderDirection Direction,
    bool NullLast = false);