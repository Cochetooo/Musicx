using Musicx.Application.Common.Interfaces.Persistence;
using Musicx.Domain.Entities;

namespace Musicx.Application.Desktop.Interfaces.Persistence;

public interface IAlbumRepository : IRepository<Album>;
public interface IArtistRepository : IRepository<Artist>;
public interface IGenreRepository : IRepository<Genre>;
public interface ILabelRepository : IRepository<Label>;
public interface IReleaseRepository : IRepository<Release>;
public interface ISongRepository : IRepository<Song>;
