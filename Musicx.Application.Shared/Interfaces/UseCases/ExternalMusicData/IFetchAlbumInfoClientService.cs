using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Artwork;

namespace Musicx.Application.Shared.Interfaces.UseCases.ExternalMusicData;

public sealed record FetchAlbumInfoRequest(
    string Name, string Artist, CancellationToken CancellationToken) : BaseRequest;

public sealed record FetchAlbumInfoResponse(
    OutArtworkSearchResult SearchResult) : BaseResponse;

public interface IFetchAlbumInfoClientService : IClientService<FetchAlbumInfoRequest, FetchAlbumInfoResponse>;