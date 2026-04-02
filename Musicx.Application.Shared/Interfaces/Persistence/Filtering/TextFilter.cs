namespace Musicx.Application.Shared.Interfaces.Persistence.Filtering;

public sealed record TextFilter(string? Value, TextMatchMode Mode = TextMatchMode.Contains)
{
    public bool IsSet => !string.IsNullOrWhiteSpace(Value);
}