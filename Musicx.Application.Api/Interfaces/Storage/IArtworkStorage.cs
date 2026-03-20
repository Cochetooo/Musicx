namespace Musicx.Application.Api.Interfaces.Storage;

public interface IArtworkStorage
{
    Task<string> SaveArtistAsync(long artistId, Stream imageStream, string contentType,
        CancellationToken token = default);

    Task<string> SaveAlbumAsync(long artistId, long albumId, Stream imageStream, string contentType,
        CancellationToken token = default);
}