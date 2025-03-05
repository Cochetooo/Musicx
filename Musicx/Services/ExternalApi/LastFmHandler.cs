using log4net;
using log4net.Repository.Hierarchy;
using Musicx.Models.ExternalApi;
using Musicx.Models.ExternalApi.LastFm;
using MusicxApi.Models;
using Album = MusicxApi.Models.Album;
using Artist = MusicxApi.Models.Artist;

namespace Musicx.Services.ExternalApi;

internal sealed class LastFmHandler(string apiUrl, string apiKey) : ExternalApiHandler(apiUrl)
{
    private readonly ILog Logger = LogManager.GetLogger(typeof(LastFmHandler));

    private readonly string ApiKey = apiKey;
    
    public override async Task<Song> FetchSong(Song song)
    {
        Logger.Debug($"⛏️ Fetching LastFm Song: {song.Title}");
        
        Logger.Warn($"⚠️ Using example artist, please fix later.");
        var artist = new Artist
        {
            Name = "Katatonia"
        };

        if (null == artist)
        {
            Logger.Warn($"⚠️ Artist has not been found.");
            return song;
        }

        var requestUrl = MakeUrl(
            LastFmApiMethod.FetchSong,
            new Dictionary<string, string>
            {
                { "api_key", ApiKey },
                { "artist", artist.Name },
                { "track", song.Title },
            }
        );

        var jsonResponse = await SendRequestAsync(requestUrl);
        var lastFmObject = jsonResponse.ToObject<LastFm_FetchSongObject>();

        return song;
    }

    public override async Task<Album> FetchAlbum(Album album)
    {
        Logger.Debug($"⛏️ Fetching LastFm Album: {album.Name}");
        
        Logger.Warn($"⚠️ Using example artist, please fix later.");
        var artist = new Artist
        {
            Name = "Katatonia"
        };

        if (null == artist)
        {
            Logger.Warn($"⚠️ Artist has not been found.");
            return album;
        }

        var requestUrl = MakeUrl(
            LastFmApiMethod.FetchAlbum,
            new Dictionary<string, string>
            {
                { "api_key", ApiKey },
                { "artist", artist.Name },
                { "album", album.Name },
            }
        );

        var jsonResponse = await SendRequestAsync(requestUrl);
        var lastFmObject = jsonResponse.ToObject<LastFm_FetchAlbumObject>();

        if (lastFmObject is not null)
        {
            album.ArtworkUrl = lastFmObject.Album.Images[3].Text;
        }
        else
        {
            Logger.Warn($"⚠️ Last Fm Album {artist.Name} - {album.Name} has not been found.");
        }
        
        return album;
    }

    public override async Task<Artist> FetchArtist(Artist artist)
    {
        Logger.Debug($"⛏️ Fetching LastFm Artist: {artist.Name}");

        var requestUrl = MakeUrl(
            LastFmApiMethod.FetchArtist,
            new Dictionary<string, string>
            {
                { "api_key", ApiKey },
                { "artist", artist.Name },
            }
        );

        var jsonResponse = await SendRequestAsync(requestUrl);
        
        return artist;
    }

    protected override string MakeUrl(string method, Dictionary<string, string> parameters)
    {
        var parametersString = string.Join("&", parameters
            .Select(parameter => $"{Uri.EscapeDataString(parameter.Key)}={Uri.EscapeDataString(parameter.Value)}"));

        return $"{ApiUrl}/?method={method}&format=json&{parametersString}";
    }

    static class LastFmApiMethod
    {
        public const string FetchSong = "track.getinfo";
        public const string FetchAlbum = "album.getinfo";
        public const string FetchArtist = "artist.getinfo";
    }
}