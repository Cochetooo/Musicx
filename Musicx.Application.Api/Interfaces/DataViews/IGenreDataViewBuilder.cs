using Musicx.Contracts.Dto.Responses.Specifics.Genres;

namespace Musicx.Application.Api.Interfaces.DataViews;

public readonly record struct GenreDataViewQuery(long GenreId, long? UserId = null);

public interface IGenreDataViewBuilder : IDataViewBuilder<GenreDataViewQuery, OutGenreDataView>;