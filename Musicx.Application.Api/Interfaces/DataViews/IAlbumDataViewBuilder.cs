using Musicx.Contracts.Dto.Responses.Specifics.Albums;

namespace Musicx.Application.Api.Interfaces.DataViews;

/// <summary>
/// Query object used by <see cref="IAlbumDataViewBuilder"/>.
/// </summary>
public readonly record struct AlbumDataViewQuery(long AlbumId, long? UserId = null);

/// <summary>
/// Builds Album page DataView payloads from persistence layer data.
/// </summary>
public interface IAlbumDataViewBuilder : IDataViewBuilder<AlbumDataViewQuery, OutAlbumDataView>;