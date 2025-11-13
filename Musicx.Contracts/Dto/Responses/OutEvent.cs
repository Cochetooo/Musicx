using Musicx.Contracts.Dto.Jsons.Events;

namespace Musicx.Contracts.Dto.Responses;

public sealed class OutEvent : BaseOutputModel
{
    public ICollection<OutEventArtist>? EventArtists { get; set; }
    public ICollection<OutEventUser>? EventUsers { get; set; }

    public ICollection<EventPrice> EventPrices { get; set; } = [];
    public ICollection<EventTicketLink> EventTicketLinks { get; set; } = [];
    
    public string? Address { get; set; }
    public DateTime? BeginDate { get; set; }
    public string? Country { get; set; }
    public string? Description { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsFestival { get; set; }
    public bool IsVisible { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PosterUrl { get; set; }
    public string? Town { get; set; }
    public string? Venue { get; set; }
    public string? ZipCode { get; set; }
}