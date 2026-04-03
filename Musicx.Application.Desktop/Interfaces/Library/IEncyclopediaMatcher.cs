using Musicx.Application.Desktop.Models;

namespace Musicx.Application.Desktop.Interfaces.Library;

public interface IEncyclopediaMatcher
{
    Task<EncyclopediaTrackMatch> MatchTrackAsync(LocalTrack localTrack, CancellationToken cancellationToken = default);
}