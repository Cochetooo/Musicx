using Musicx.Contracts.Dto.Responses.Specifics.Users;

namespace Musicx.Application.Api.Interfaces.DataViews;

/// <summary>
/// Query object used by <see cref="IUserDataViewBuilder"/>.
/// </summary>
public readonly record struct UserDataViewQuery(long UserId, long? CurrentUserId = null);

/// <summary>
/// Builds User page DataView payloads from persistence layer data.
/// </summary>
public interface IUserDataViewBuilder : IDataViewBuilder<UserDataViewQuery, OutUserDataView>;