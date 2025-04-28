using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Domain.Models;

namespace Musicx.Application.Desktop.Interfaces.UseCases.LocalLibrary;

public sealed record PersistLocalSongsRequest(
    IEnumerable<Song> Songs,
    IEnumerable<Album> Albums,
    IEnumerable<Artist> Artists) : BaseRequest;

public sealed record PersistLocalSongsResponse : BaseResponse;

public interface IPersistLocalSongsUseCase : IUseCase<PersistLocalSongsRequest, PersistLocalSongsResponse>;