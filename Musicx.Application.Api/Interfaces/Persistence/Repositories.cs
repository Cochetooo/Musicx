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
public interface ISongRepository : IRepository<InSong, OutSong>;
