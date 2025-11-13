using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Specifics;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Dto.Responses.Specifics.Ratings;
using Musicx.Contracts.Enums;

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
    Task<List<OutAlbum>> FindByChart(
        AlbumChartQuery query);
}
public interface IArtistRepository : IRepository<InArtist, OutArtist>;
public interface IEventRepository : IRepository<InEvent, OutEvent>;
public interface IEventArtistRepository : IRepository<InEventArtist, OutEventArtist>;
public interface IEventUserRepository : IRepository<InEventUser, OutEventUser>;
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

public interface ITagRepository : IRepository<InTag, OutTag>;

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
        long? artistId = null,
        bool? filterExact = null,
        double? filterSimilitude = 0.4,
        string? filter = null,
        string? order = null);
    Task<OutUserAlbumAttribute?> FindOneAlbumFromUserAsync(long userId, long albumId);
    Task<OutUserRatingStats> GetUserRatingStatsAsync(long userId);
}

public interface IUserAlbumTagRepository : IRepository<InUserAlbumTag, OutUserAlbumTag>
{
    Task<IReadOnlyList<OutUserAlbumAttribute>> FindByAlbumIdAsync(long albumId, long? userId);
}
