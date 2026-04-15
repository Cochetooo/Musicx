using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Application.Web.ViewModels.Album;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses.Specifics.Albums;
using Musicx.Contracts.Dto.Responses.User;
using Musicx.Infrastructure.API.Persistence.Mappers;

namespace Musicx.Infrastructure.Web.Services.Album;

public sealed class AlbumViewService
{
    private readonly IApiClient _api;

    public AlbumViewService(IApiClient api)
    {
        _api = api;
    }

    public async Task<AlbumViewModel?> LoadAlbum(long albumId, long? userId)
    {
        var data = await _api.GetDataViewAsync<OutAlbumDataView>(
            "albums",
            albumId,
            new Dictionary<string, object?>
            {
                { "userId", userId }
            });

        if (data is null) return null;

        return new AlbumViewModel
        {
            Album = data.Album,
            Songs = data.Songs.ToList(),
            PreviousAlbum = data.PreviousAlbum,
            NextAlbum = data.NextAlbum,
            UserAttribute = data.CurrentUserAttribute?.ToRaw() ?? new InUserAlbumAttribute
            {
                AlbumId = albumId,
                UserId = userId ?? 0
            },
            UserSongAttributes = data.CurrentUserSongAttributes,
            SimilarAlbums = data.SimilarAlbums.ToList(),
            FactorAverages = ComputeFactorAverages(data.Ratings?.Items ?? [])
        };
    }

    public Dictionary<string, short> ComputeFactorAverages(IEnumerable<OutUserAlbumAttribute> ratings)
    {
        var result = new Dictionary<string, short>();

        void Compute(string key, Func<OutUserAlbumAttribute, short?> selector)
        {
            var values = ratings.Where(x => selector(x).HasValue)
                                .Select(x => selector(x)!.Value)
                                .ToList();

            if (values.Count > 0)
                result[key] = (short)Math.Round(values.Average(x => (int)x));
        }

        Compute("Production", x => x.ProductionRating);
        Compute("Lyrics", x => x.LyricsRating);
        Compute("Instrumentation", x => x.InstrumentationRating);
        Compute("Vocals", x => x.VocalsRating);
        Compute("Atmosphere", x => x.AtmosphereRating);
        Compute("Originality", x => x.OriginalityRating);

        return result;
    }

    public short ComputeRatingFromFactors(InUserAlbumAttribute attr)
    {
        var values = new short?[]
        {
            attr.ProductionRating,
            attr.LyricsRating,
            attr.InstrumentationRating,
            attr.VocalsRating,
            attr.AtmosphereRating,
            attr.OriginalityRating
        };

        var valid = values.Where(v => v.HasValue).Select(v => v!.Value).ToList();

        return valid.Count == 0
            ? (short)0
            : (short)Math.Round(valid.Average(v => (int)v));
    }
}