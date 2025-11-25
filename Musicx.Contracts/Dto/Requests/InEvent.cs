using System.Text.Json.Nodes;
using Musicx.Contracts.Dto.Jsons.Events;

namespace Musicx.Contracts.Dto.Requests;

public sealed class InEvent : BaseInputModel
{
    // Optional Relationships
    public string? EventPrices { get; set; }
    public string? EventTicketLinks { get; set; }

    // Required Columns
    public bool IsFestival { get; set; }
    public bool IsVisible { get; set; }
    public string Name { get; set; } = string.Empty;
    
    // Optional Columns
    public string? Address { get; set; }
    public DateTime? BeginDate { get; set; }
    public string? Country { get; set; }
    public string? Description { get; set; }
    public DateTime? EndDate { get; set; }
    public string? PosterUrl { get; set; }
    public string? Town { get; set; }
    public string? Venue { get; set; }
    public string? ZipCode { get; set; }
}