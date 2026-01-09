namespace Musicx.Contracts.Dto.ValueObjects.Events;

public sealed class EventTicketLink
{
    public string Name { get; set; }
    public bool Official { get; set; }
    public string Url { get; set; }
}