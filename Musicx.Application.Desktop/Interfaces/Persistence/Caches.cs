using Musicx.Application.Common.Interfaces.Persistence;
using Musicx.Domain.Entities;

namespace Musicx.Application.Desktop.Interfaces.Persistence;

public interface ISongCache : ICache<Song>;
public interface IArtistCache : ICache<Artist>;
public interface IAlbumCache : ICache<Album>;
public interface IGenreCache : ICache<Genre>;
public interface IReleaseCache : ICache<Release>;
public interface ILabelCache : ICache<Label>;