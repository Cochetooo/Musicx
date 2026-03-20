using Musicx.Application.Api.Interfaces.Storage;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Musicx.Infrastructure.API.Storage;

public sealed class LocalArtworkStorage(IPublicFileRoot root) : IArtworkStorage
{
    private readonly IPublicFileRoot _root = root;

    public Task<string> SaveArtistAsync(long artistId, Stream imageStream, string contentType, CancellationToken token = default)
        => SaveAsync(Path.Combine("Artists", artistId.ToString()), "Artwork.webp", imageStream, token);

    public Task<string> SaveAlbumAsync(long artistId, long albumId, Stream imageStream, string contentType, CancellationToken token = default)
        => SaveAsync(Path.Combine("Artists", artistId.ToString(), "Albums", albumId.ToString()), "Artwork.webp", imageStream, token);

    private async Task<string> SaveAsync(string relativeFolder, string fileName, Stream imageStream, CancellationToken token)
    {
        var basePath = _root.GetWebRootPath();
        var folder = Path.Combine(basePath, relativeFolder);
        Directory.CreateDirectory(folder);

        var filePath = Path.Combine(folder, fileName);

        await using var input = new MemoryStream();
        await imageStream.CopyToAsync(input, token);
        input.Position = 0;

        using var image = await Image.LoadAsync(input, token);
        while (image.Frames.Count > 1)
        {
            image.Frames.RemoveFrame(image.Frames.Count - 1);
        }

        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max,
            Size = new Size(1400, 1400)
        }));

        await image.SaveAsync(filePath, new WebpEncoder { Quality = 92, FileFormat = WebpFileFormatType.Lossy }, token);

        return $"{_root.GetBaseUrl()}/{relativeFolder.Replace(Path.DirectorySeparatorChar, '/')}/{fileName}";
    }
}