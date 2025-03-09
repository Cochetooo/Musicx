using log4net;
using Microsoft.Extensions.Logging;
using Musicx.Core.Interfaces;
using Musicx.Infrastructure.Managers;
using Musicx.Core.Models;
using Musicx.Core.Models.Enums;
using Musicx.Infrastructure.Repositories;
using TagLib;
using ILogger = Musicx.Core.Logging.ILogger;
using ILoggerFactory = Musicx.Core.Logging.ILoggerFactory;

namespace Musicx.Infrastructure.Services.Audio;

public class ReadAudioFileService(ISongManager songManager,
    IAlbumManager albumManager,
    IArtistManager artistManager,
    IGenreManager genreManager,
    ILabelManager labelManager,
    ILoggerFactory loggerFactory) : IService
{
    private readonly ILogger Logger = loggerFactory.CreateLogger(typeof(ReadAudioFileService));
    
    public async Task<Song> ExecuteAsync(string filePath)
    {
        Logger.Debug("⛏️ Executing ReadAudioFile");
        
        var file = TagLib.File.Create(filePath);
    
        Logger.Warn("⚠️ Audio Format set to MP3 per default. Fix later");
        var song = await ReadSong(file);
        
        Logger.Info($"Track : {song.BitRate} Kbps | {song.Duration}s | {song.Filepath} | {song.SampleRate} Hz | {song.Title}");
        
        Logger.Debug("✅ ReadAudioFile success");

        return song;
    }

    private async Task<Song> ReadSong(TagLib.File file)
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
            Title = file.Tag.Title,
            TrackNumber = file.Tag.Track
        };
        
        var existingSongs = await songManager.FindAll(filter:
            s => s.Duration == song.Duration && s.Title == song.Title && s.TrackNumber == song.TrackNumber);

        var existingSong = existingSongs.FirstOrDefault();
        if (null != existingSong)
        {
            Logger.Info($"ℹ️ Found existing song with id {existingSong.Id}.");
            song.Id = existingSong.Id;
            
            song.ArtistId = existingSong.ArtistId;
            song.AlbumId = existingSong.AlbumId;
            song.GenreIds = existingSong.GenreIds;
            song.InfluenceGenreIds = existingSong.InfluenceGenreIds;
        }

        return song;
    }

    private async Task<Album> ReadAlbum(TagLib.File file, Song song)
    {
        Logger.Debug("➕ Reading Album Data");
        
        Logger.Warn("⚠️ Artwork Url, Catalog Number, Release Date and Album Type not set. Please fix later.");

        var album = new Album
        {
            ArtworkUrl = "",
            CatalogNumber = "",
            DiscTotal = file.Tag.DiscCount,
            Name = file.Tag.Album,
            ReleaseDate = DateTime.MinValue,
            TrackTotal = file.Tag.TrackCount,
        };

        return album;
    }

    private void ReadArtist(in TagLib.File file, Song song)
    {
        Logger.Debug("➕ Reading Artist Data");
    }

    private void ReadGenres(in TagLib.File file, Song song)
    {
        Logger.Debug("➕ Reading Genres Data");
    }

    private void ReadLabel(in TagLib.File file, Song song)
    {
        Logger.Debug("➕ Reading Label Data");
    }
}
