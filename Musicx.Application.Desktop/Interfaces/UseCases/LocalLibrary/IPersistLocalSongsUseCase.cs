using Musicx.Application.Common.Interfaces.Common;

namespace Musicx.Application.Desktop.Interfaces.UseCases.LocalLibrary;

public sealed record PersistLocalSongsRequest(
    IEnumerable<LocalSongDto> LocalSongs) : BaseRequest;

public sealed record PersistLocalSongsResponse : BaseResponse;

public interface IPersistLocalSongsUseCase : IUseCase<PersistLocalSongsRequest, PersistLocalSongsResponse>;