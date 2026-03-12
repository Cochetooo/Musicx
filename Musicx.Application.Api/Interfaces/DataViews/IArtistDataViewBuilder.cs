using Musicx.Contracts.Dto.Responses.Specifics.Artists;

namespace Musicx.Application.Api.Interfaces.DataViews;

public readonly record struct ArtistDataViewQuery(long ArtistId, long? UserId = null);

public interface IArtistDataViewBuilder : IDataViewBuilder<ArtistDataViewQuery, OutArtistDataView>;