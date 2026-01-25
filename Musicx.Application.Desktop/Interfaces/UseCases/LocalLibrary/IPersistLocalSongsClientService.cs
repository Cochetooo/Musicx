using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Contracts.Dto.Requests.Artist;
using Musicx.Contracts.Dto.Requests.Song;


namespace Musicx.Application.Desktop.Interfaces.UseCases.LocalLibrary;

public sealed record PersistLocalSongsRequest(
    IEnumerable<InSong> Songs,
    IEnumerable<InAlbum> Albums,
    IEnumerable<InArtist> Artists) : BaseRequest;

public sealed record PersistLocalSongsResponse : BaseResponse;

public interface IPersistLocalSongsClientService : IClientService<PersistLocalSongsRequest, PersistLocalSongsResponse>;