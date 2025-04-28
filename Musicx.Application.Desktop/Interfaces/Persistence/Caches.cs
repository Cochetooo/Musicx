using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Domain.Models;

namespace Musicx.Application.Desktop.Interfaces.Persistence;

public interface ISongCache : ICache<Song>;
public interface IArtistCache : ICache<Artist>;
public interface IAlbumCache : ICache<Album>;
public interface IGenreCache : ICache<Genre>;
public interface IReleaseCache : ICache<Release>;
public interface ILabelCache : ICache<Label>;