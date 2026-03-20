using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Artwork;


namespace Musicx.Application.Shared.Interfaces.UseCases.ExternalMusicData;

public sealed record FetchArtistInfoRequest(
    string Name, CancellationToken CancellationToken) : BaseRequest;

public sealed record FetchArtistInfoResponse(
    OutArtworkSearchResult SearchResult) : BaseResponse;

public interface IFetchArtistInfoClientService : IClientService<FetchArtistInfoRequest, FetchArtistInfoResponse>;