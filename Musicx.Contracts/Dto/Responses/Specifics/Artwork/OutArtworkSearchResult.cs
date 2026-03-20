namespace Musicx.Contracts.Dto.Responses.Specifics.Artwork;

public sealed class OutArtworkSearchResult
{
    public IReadOnlyList<OutArtworkCandidate> Candidates { get; set; } = [];
}