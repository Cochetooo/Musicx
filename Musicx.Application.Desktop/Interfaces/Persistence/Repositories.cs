using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;


namespace Musicx.Application.Desktop.Interfaces.Persistence;

public interface IAlbumRepository : IRepository<InAlbum, OutAlbum>;
public interface IArtistRepository : IRepository<InArtist, OutArtist>;
public interface IGenreRepository : IRepository<InGenre, OutGenre>;
public interface ILabelRepository : IRepository<InLabel, OutLabel>;
public interface IReleaseRepository : IRepository<InRelease, OutRelease>;
public interface ISongRepository : IRepository<InSong, OutSong>;
