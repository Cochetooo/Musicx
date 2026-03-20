namespace Musicx.Contracts.Dto.Responses.Specifics.Artwork;

public sealed class OutArtworkCandidate
{
    public string Url { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public double Score { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public bool IsAnimated { get; set; }
}