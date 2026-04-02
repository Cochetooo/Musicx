using Musicx.Application.Api.Interfaces.Caching;

namespace Musicx.Infrastructure.API.Caching;

public sealed class DataViewCacheKeyFactory : IDataViewCacheKeyFactory
{
    public string BuildAlbumDataViewKey(long albumId, long? userId = null)
    {
        var scope = userId.HasValue ? $"user:{userId.Value}" : "anon";
        return $"dataview:album:{albumId}:{scope}";
    }
    
    public string BuildArtistDataViewKey(long artistId, long? userId = null)
    {
        var scope = userId.HasValue ? $"user:{userId.Value}" : "anon";
        return $"dataview:artist:{artistId}:{scope}";
    }
    
    public string BuildGenreDataViewKey(long genreId, long? userId = null)
    {
        var scope = userId.HasValue ? $"user:{userId.Value}" : "anon";
        return $"dataview:genre:{genreId}:{scope}";
    }

    public string BuildUserDataViewKey(long userId, long? currentUserId = null)
    {
        var scope = currentUserId.HasValue ? $"viewer:{currentUserId.Value}" : "anon";
        return $"dataview:user:{userId}:{scope}";
    }
}