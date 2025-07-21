using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Domain.Models;

namespace Musicx.Application.Api.Interfaces.Persistence;

public interface IAlbumRepository : IRepository<Album>
{
    Task<List<Album>> FindByArtistIdAsync(long artistId, IQuerySpecification<Album>? albumQuerySpecification = null);
}
public interface IArtistRepository : IRepository<Artist>;
public interface IGenreRepository : IRepository<Genre>;
public interface ILabelRepository : IRepository<Label>;
public interface IReleaseRepository : IRepository<Release>;
public interface ISongRepository : IRepository<Song>;
