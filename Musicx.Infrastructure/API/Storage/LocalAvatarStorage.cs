using Musicx.Application.Api.Interfaces.Storage;
using Musicx.Infrastructure.Shared.Models.ExternalMusicData;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Musicx.Infrastructure.API.Storage;

public sealed class LocalAvatarStorage : IAvatarStorage
{
    private readonly IPublicFileRoot _root;

    public LocalAvatarStorage(IPublicFileRoot root)
    {
        _root = root;
    }
    
    public async Task<string> SaveAsync(long userId, Stream imageStream, string contentType, CancellationToken token = default)
    {
        var basePath = _root.GetPath();
        var folder = Path.Combine(basePath, "Avatars", userId.ToString());
        Directory.CreateDirectory(folder);
        
        var filePath = Path.Combine(folder, "avatar.webp");

        using var image = await Image.LoadAsync(imageStream, token);
        image.Mutate(x => x.Resize(256, 256));
        
        await image.SaveAsync(filePath, new WebpEncoder { Quality = 85 }, token);

        return $"{_root.GetBaseUrl()}/Avatars/{userId}/avatar.webp";
    }

    public Task DeleteAsync(long userId, CancellationToken token = default)
    {
        var path = Path.Combine(_root.GetPath(), "Avatars", userId.ToString());
        
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
        
        return Task.CompletedTask;
    }
}