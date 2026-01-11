using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Api.Interfaces.Persistence.Repositories.User;

public interface IUserAlbumTagRepository : IRepository<InUserAlbumTag, OutUserAlbumTag>
{
    Task<IReadOnlyList<OutUserAlbumAttribute>> FindByAlbumIdAsync(long albumId, long? userId);
}