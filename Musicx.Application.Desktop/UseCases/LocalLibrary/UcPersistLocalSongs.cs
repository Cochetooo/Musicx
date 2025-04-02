using System.Collections.Concurrent;
using Musicx.Application.Common.Interfaces.Common;
using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Application.Desktop.Interfaces.UseCases.LocalLibrary;
using Musicx.Domain.Entities;

namespace Musicx.Application.Desktop.UseCases.LocalLibrary;

public class UcPersistLocalSongs(
    ILoggerFactory loggerFactory,
    ISongRepository songRepository,
    IAlbumRepository albumRepository,
    IArtistRepository artistRepository) : IPersistLocalSongsUseCase
{
    private readonly ILogger<UcPersistLocalSongs> _logger = loggerFactory.CreateLogger<UcPersistLocalSongs>();
    
    public async Task<PersistLocalSongsResponse> ExecuteAsync(PersistLocalSongsRequest request)
    {
        _logger.Debug("⛏️ Execute : PersistLocalSongs");

        var songs = new ConcurrentBag<Song>();
        var albums = new ConcurrentBag<Album>();
        var artists = new ConcurrentBag<Artist>();
        
        _logger.Warn("⚠️ Not considering genres, albums and artists for now, please fix later.");

        var tasks = request.LocalSongs.Select(async localSong =>
        {
            var song = new Song();
            var album = new Album();
            Artist artist = localSong.ArtistDiscriminator == "Band" 
                ? new BandArtist()
                : new PersonArtist();
            Artist albumArtist = localSong.AlbumArtistDiscriminator == "Band" 
                ? new BandArtist()
                : new PersonArtist();
            
            song.CreatedAt = DateTime.Now;
            song.UpdatedAt = DateTime.Now;
            song.DiscNumber = localSong.SongDiscNumber;
            song.Duration = localSong.SongDuration;
            song.Lyrics = localSong.SongLyrics;
            song.Title = localSong.SongTitle;
            song.TrackNumber = localSong.SongTrackNumber;
            
            songs.Add(song);
        }).ToList();
        
        await Task.WhenAll(tasks);

        await songRepository.SaveAllAsync(songs);
        await albumRepository.SaveAllAsync(albums);
        await artistRepository.SaveAllAsync(artists);
        
        _logger.Debug("✅ PersistLocalSongs success!");

        return new PersistLocalSongsResponse();
    }

    public PersistLocalSongsResponse Execute(PersistLocalSongsRequest request)
    {
        throw new NotImplementedException();
    }
}