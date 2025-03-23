using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Core.Models;
using Musicx.Core.Models.Enums;
using Musicx.Infrastructure.Helpers;
using Musicx.Infrastructure.Managers;
using File = TagLib.File;
using ILoggerFactory = Musicx.Core.Logging.ILoggerFactory;

namespace Musicx.Infrastructure.Services.Audio;

public class ReadAudioFileService(ISongManager songManager,
    IAlbumManager albumManager,
    IArtistManager artistManager,
    IGenreManager genreManager,
    ILabelManager labelManager,
    ILoggerFactory loggerFactory) : IService
{
    private readonly ILogger<ReadAudioFileService> Logger = loggerFactory.CreateLogger<ReadAudioFileService>();

    public async Task<Song> ExecuteAsync(string filePath)
    {
        Logger.Debug("⛏️ Executing ReadAudioFile");
        
        var file = File.Create(filePath);
    
        Logger.Warn("⚠️ Audio Format set to MP3 per default. Fix later");
        var song = await ReadSong(file);
        
        Logger.Info($"Track : {song.BitRate} Kbps | {song.Duration}s | {song.Filepath} | {song.SampleRate} Hz | {song.Title}");
        
        Logger.Debug("✅ ReadAudioFile success");

        return song;
    }

    private async Task<Song> ReadSong(File file)
    {
        Logger.Debug("➕ Reading Song Data");
        
        var song = new Song
        {
            AudioFormat = AudioFormatType.Mp3,
            BitRate = file.Properties.AudioBitrate,
            DiscNumber = file.Tag.Disc,
            Duration = (long) file.Properties.Duration.TotalSeconds,
            Filepath = file.Name,
            Lyrics = file.Tag.Lyrics,
            SampleRate = file.Properties.AudioSampleRate,
            Title = string.IsNullOrEmpty(file.Tag.Title) ? "Unknown" : file.Tag.Title,
            TrackNumber = file.Tag.Track
        };
        
        var existingSong = await songManager.FindExisting(song);
        if (null != existingSong)
        {
            Logger.Debug($"ℹ️ Found existing song with id {existingSong.Id}.");
            song.Id = existingSong.Id;
            
            song.ArtistId = existingSong.ArtistId;
            song.AlbumId = existingSong.AlbumId;
            song.GenreIds = existingSong.GenreIds;
            song.InfluenceGenreIds = existingSong.InfluenceGenreIds;
        }
        else
        {
            var album = await ReadAlbum(file, song);
            song.AlbumId = album.Id;
            
            var artist = await ReadArtist(file);
            song.ArtistId = artist.Id;
            
            Logger.Debug($"ℹ️ Creating new song, album id : {album.Id} artist id: {artist.Id}");
        }

        return song;
    }

    private async Task<Album> ReadAlbum(File file, Song song)
    {
        Logger.Debug("➕ Reading Album Data");
        
        var album = new Album
        {
            ArtworkUrl = AudioTagHelper.ReadCustomTag(song.Filepath, "ALBUM ARTWORK URL"),
            CatalogNumber = AudioTagHelper.ReadCustomTag(song.Filepath, "CATALOGNUMBER"),
            DiscTotal = file.Tag.DiscCount,
            Name = string.IsNullOrEmpty(file.Tag.Album) ? "Unknown" : file.Tag.Album,
            ReleaseDate = new DateTime((int) file.Tag.Year, 1, 1),
            TrackTotal = file.Tag.TrackCount,
        };
        
        Enum.TryParse(typeof(ReleaseType), AudioTagHelper.ReadCustomTag(song.Filepath, "ALBUM TYPE"), true,
            out var result);

        if (result is ReleaseType releaseType)
        {
            album.ReleaseType = releaseType;
        }
        
        var existingAlbum = await albumManager.FindExisting(album);

        if (null != existingAlbum)
        {
            Logger.Info($"ℹ️ Found existing album with id {existingAlbum.Id}.");
            album.Id = existingAlbum.Id;
            
            album.ArtistId = existingAlbum.ArtistId;
            album.GenreIds = existingAlbum.GenreIds;
            album.InfluenceGenreIds = existingAlbum.InfluenceGenreIds;
            album.LabelId = existingAlbum.LabelId;
        }
        else
        {
            album.Id = await albumManager.Save(album);
        }

        return album;
    }

    private async Task<Artist> ReadArtist(File file)
    {
        Logger.Debug("➕ Reading Artist Data");
        
        Logger.Warn("⚠️ Choosing Band artist to create new artist. Please fix later.");
        var artist = new BandArtist
        {
            ArtworkUrl = "",
            Country = "",
            Name = string.IsNullOrEmpty(file.Tag.FirstAlbumArtist) ? "Unknown" : file.Tag.FirstAlbumArtist,
        };

        var existingArtist = await artistManager.FindExisting(artist);

        if (null != existingArtist)
        {
            Logger.Info($"ℹ️ Found existing artist with id {existingArtist.Id}.");
            artist.Id = existingArtist.Id;
        }
        else
        {
            artist.Id = await artistManager.Save(artist);
        }
        
        return artist;
    }

    private void ReadGenres(in File file, Song song)
    {
        Logger.Debug("➕ Reading Genres Data");
    }

    private void ReadLabel(in File file, Song song)
    {
        Logger.Debug("➕ Reading Label Data");
    }
}
