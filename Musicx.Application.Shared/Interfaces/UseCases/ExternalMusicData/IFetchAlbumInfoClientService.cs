using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Shared.Interfaces.UseCases.ExternalMusicData;

public sealed record FetchAlbumInfoRequest(
    string Name, string Artist, CancellationToken CancellationToken) : BaseRequest;

public sealed record FetchAlbumInfoResponse(
    OutAlbum? Album) : BaseResponse;

public interface IFetchAlbumInfoClientService : IClientService<FetchAlbumInfoRequest, FetchAlbumInfoResponse>;