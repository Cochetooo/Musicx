using System.Reflection.Emit;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Genre;


namespace Musicx.Application.Desktop.Interfaces.Persistence;

public interface ISongCache : ICache<OutSong>;
public interface IArtistCache : ICache<OutArtist>;
public interface IAlbumCache : ICache<OutAlbum>;
public interface IGenreCache : ICache<OutGenre>;
public interface IReleaseCache : ICache<OutRelease>;
public interface ILabelCache : ICache<OutLabel>;