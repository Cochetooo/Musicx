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
        long skip = 0, 
        long take = 100,
        string? order = null,
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
        long skip = 0,
        long take = 100,
        string? order = null);
    Task<IReadOnlyList<OutUserAlbumAttribute>> FindByUserIdAsync(long userId,
        long skip = 0,
        long take = 100,
        bool? filterExact = null,
        double? filterSimilitude = 0.4,
        string? filter = null,
        string? order = null);
    Task<OutUserAlbumAttribute?> FindOneAlbumFromUserAsync(long userId, long albumId);
}

public interface IUserAlbumTagRepository : IRepository<InUserAlbumTag, OutUserAlbumTag>
{
    Task<IReadOnlyList<OutUserAlbumAttribute>> FindByAlbumIdAsync(long albumId, long? userId);
}
