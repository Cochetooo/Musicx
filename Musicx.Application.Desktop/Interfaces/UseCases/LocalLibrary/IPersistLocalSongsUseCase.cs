using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Contracts.Dto.Requests;


namespace Musicx.Application.Desktop.Interfaces.UseCases.LocalLibrary;

public sealed record PersistLocalSongsRequest(
    IEnumerable<InSong> Songs,
    IEnumerable<InAlbum> Albums,
    IEnumerable<InArtist> Artists) : BaseRequest;

public sealed record PersistLocalSongsResponse : BaseResponse;

public interface IPersistLocalSongsUseCase : IUseCase<PersistLocalSongsRequest, PersistLocalSongsResponse>;