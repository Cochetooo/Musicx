using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Api.Interfaces.Persistence;

public interface IAlbumRepository : IRepository<InAlbum, OutAlbum>
{
    Task<List<OutAlbum>> FindByArtistIdAsync(long artistId, 
        IQuerySpecification<InAlbum>? albumQuerySpecification = null);
    Task<List<OutAlbum>> FindByGenreIdAsync(long genreId, 
        int genreOptions,
        int skip = 0, 
        int take = 100,
        IQuerySpecification<InAlbum>? albumQuerySpecification = null);
}
public interface IArtistRepository : IRepository<InArtist, OutArtist>;
public interface IGenreRepository : IRepository<InGenre, OutGenre>;
public interface ILabelRepository : IRepository<InLabel, OutLabel>;
public interface IReleaseRepository : IRepository<InRelease, OutRelease>;

public interface ISongRepository : IRepository<InSong, OutSong>
{
    Task<List<OutSong>> FindByAlbumIdAsync(long albumId,
        IQuerySpecification<InSong>? songQuerySpecification = null);
}

public interface IPermissionRepository : IRepository<InPermission, OutPermission>;

public interface IRoleRepository : IRepository<InRole, OutRole>;

public interface IUserRepository : IRepository<InUser, OutUser>
{
    Task<OutUser?> FindByEmailAsync(string email,
        IQuerySpecification<InUser>? userQuerySpecification = null);
}

public interface IUserAlbumAttrsRepository : IRepository<InUserAlbumAttribute, OutUserAlbumAttribute>
{
    Task<long> CountByAlbumIdAsync(long albumId);
    Task<long> CountByUserIdAsync(long userId);
    Task DeleteAsync(long userId, long albumId);
    Task<IReadOnlyList<OutUserAlbumAttribute>> FindByAlbumIdAsync(long albumId,
        int skip = 0,
        int take = 100);
    Task<IReadOnlyList<OutUserAlbumAttribute>> FindByUserIdAsync(long userId,
        int skip = 0,
        int take = 100,
        string? filter = null);
    Task<OutUserAlbumAttribute?> FindOneAlbumFromUserAsync(long userId, long albumId);
}

public interface IUserAlbumTagRepository : IRepository<InUserAlbumTag, OutUserAlbumTag>
{
    Task<IReadOnlyList<OutUserAlbumAttribute>> FindByAlbumIdAsync(long albumId, long? userId);
}
