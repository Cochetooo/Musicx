using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Contracts.Dto.Responses;


namespace Musicx.Application.Shared.Interfaces.UseCases.ExternalMusicData;

public sealed record FetchArtistInfoRequest(
    string Name, CancellationToken CancellationToken) : BaseRequest;

public sealed record FetchArtistInfoResponse(
    OutArtist? Artist) : BaseResponse;

public interface IFetchArtistInfoUseCase : IUseCase<FetchArtistInfoRequest, FetchArtistInfoResponse>;