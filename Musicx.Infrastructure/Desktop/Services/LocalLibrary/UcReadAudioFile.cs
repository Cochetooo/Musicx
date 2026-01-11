using ATL;
using Microsoft.Extensions.Logging;
using Musicx.Application.Desktop.Interfaces.UseCases.LocalLibrary;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Contracts.Dto.Requests.Artist;
using Musicx.Contracts.Dto.Requests.Song;
using Musicx.Contracts.Enums;


namespace Musicx.Infrastructure.Desktop.Services.LocalLibrary;

public class UcReadAudioFile(
    ILoggerProvider loggerProvider) : IReadAudioFileUseCase
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UcReadAudioFile));
    
    public async Task<ReadAudioFileResponse> ExecuteAsync(ReadAudioFileRequest request)
    {
        _logger.LogDebug("⛏️ Execute : ReadAudioFile");

        var track = new Track(request.FilePath);
        
        InSong song;
        InAlbum album;
        InArtist artist;
        
        _logger.LogWarning("⚠️ Lyrics synchronization and Audio Format are not yet supported.");

        // If AutoCheck is on, we retrieve information with the API
        if (request.AutoCheck)
        {
            throw new NotImplementedException("Auto check not implemented");
        }
        else
        {
            song = new InSong
            {
                DiscNumber = track.DiscNumber,
                Duration = track.Duration,
                //FilePath = request.FilePath,
                Lyrics = track.Lyrics.UnsynchronizedLyrics,
                Title = track.Title,
                TrackNumber = track.TrackNumber,
                
                /*BitRate = (ushort) track.Bitrate,
                Format = AudioFormatType.Unknown,
                SampleRate = track.SampleRate,
                VolumeModifier = 0.0*/
            };

            album = new InAlbum
            {
                ArtworkUrl = track.AdditionalFields.GetValueOrDefault("AlbumArtworkUrl"),
                DiscTotal = track.DiscTotal,
                Name = track.Album,
                OriginalReleaseDate = track.OriginalReleaseDate,
                ReleaseType = EnumHelper.ParseOrDefault(track.AdditionalFields.GetValueOrDefault("ReleaseType", "Unknown"), ReleaseType.Unknown),
                TrackTotal = track.TrackTotal
            };

            artist = new InArtist
            {
                ArtworkUrl = track.AdditionalFields.GetValueOrDefault("ArtistArtworkUrl"),
                CurrentCountry = track.AdditionalFields.GetValueOrDefault("ArtistCountry"),
                Name = track.Artist
            };
        }

        _logger.LogDebug("✅ ReadAudioFile success!");
        return new ReadAudioFileResponse(
            song, 
            album, 
            artist);
    }

    public ReadAudioFileResponse Execute(ReadAudioFileRequest request)
    {
        throw new NotImplementedException();
    }
}