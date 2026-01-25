using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Application.Desktop.Interfaces.UseCases.LocalLibrary;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Contracts.Dto.Requests.Artist;
using Musicx.Contracts.Dto.Requests.Song;


namespace Musicx.Application.Desktop.UseCases.LocalLibrary;

/// <summary>
/// Import all local songs from one or many folders.
/// </summary>
/// <since>0.3.0</since>
public sealed class UcImportLocalSongs(
    IReadAudioFileClientService readAudioFile,
    IPersistLocalSongsClientService persistLocalSongs,
    ILoggerProvider loggerProvider
    ) : IImportLocalSongsClientService
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UcImportLocalSongs));
    
    /// <summary>
    /// Retrieve audio files from all the library folder paths, then import each files found
    /// into database with multithreaded tasks.
    /// </summary>
    /// <since>0.3.0</since>
    public async Task<ImportLocalSongsResponse> ExecuteAsync(ImportLocalSongsRequest request)
    {
        _logger.LogDebug("⛏️ Execute : ImportLocalSongs");
        
        // Retrieve audio files from all the library folder paths.
        var audioFiles = GetFilesWithSpecifiedFormats(request.FolderPaths, request.AcceptedFormats);
        var fileNumbers = audioFiles.Count;

        // If there is no file found, no need to pursue the operation.
        if (0 == fileNumbers)
        {
            _logger.LogWarning($"⚠️ No audio file found in folders {string.Join(',', request.FolderPaths)}");
            request.ProgressListener.UpdateProgress(1, 1, null);
            return new ImportLocalSongsResponse(0, 0, 0);
        }
        
        // Initialize counters
        request.ProgressListener.UpdateProgress(0, fileNumbers, null);
        var fileProgressCount = 0;
        var successfulFileCount = 0;
        var failedFileCount = 0;
        
        var songs = new ConcurrentBag<InSong>();
        var albumsByName = new ConcurrentDictionary<string, InAlbum>();
        var artistsByName = new ConcurrentDictionary<string, InArtist>();

        // Create multithreaded tasks
        var tasks = audioFiles.Select(async filePath =>
        {
            try
            {
                var readAudioFileResponse = await readAudioFile.ExecuteAsync(
                    new ReadAudioFileRequest(filePath, request.AutoCheck)
                );

                var song = readAudioFileResponse.Song;
                var album = readAudioFileResponse.Album;
                var artist = readAudioFileResponse.Artist;
                
                _logger.LogInformation($"➕ Adding : {artist.Name} - {album.Name} - {song.Title}");

                if (!artistsByName.TryGetValue(artist.Name, out var existingArtist))
                {
                    _logger.LogDebug($"ℹ️ Creating new artist: {artist.Name}");
                    artistsByName[artist.Name] = artist;
                }
                else
                {
                    artist = existingArtist;
                }

                if (!albumsByName.TryGetValue(album.Name, out var existingAlbum))
                {
                    _logger.LogDebug($"ℹ️ Creating new album: {album.Name}");
                    albumsByName[album.Name] = album;
                }
                else
                {
                    album = existingAlbum;
                }

                song.ArtistId = artist.Id;
                song.AlbumId = album.Id;
                album.ArtistId = artist.Id;
                
                songs.Add(song);
                
                Interlocked.Increment(ref successfulFileCount);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"❌ Could not read audio file: {filePath}", ex);
                Interlocked.Increment(ref failedFileCount);
            }
            
            Interlocked.Increment(ref fileProgressCount);
            request.ProgressListener.UpdateProgress(fileProgressCount, fileNumbers, null);
        }).ToList();
        
        await Task.WhenAll(tasks);
        
        _logger.LogInformation($"ℹ️ Success: {successfulFileCount} | Failed: {failedFileCount} | TOTAL: {fileProgressCount}");

        await persistLocalSongs.ExecuteAsync(new PersistLocalSongsRequest(
            songs, 
            albumsByName.Values, 
            artistsByName.Values));
        
        request.ProgressListener.UpdateProgress(0, 0, null);
        _logger.LogDebug("✅ ImportLocalSongs success!");

        return new ImportLocalSongsResponse
        (
            TotalFileCount: fileNumbers,
            SuccessfulFileCount: successfulFileCount,
            FailedFileCount: failedFileCount
        );
    }

    public ImportLocalSongsResponse Execute(ImportLocalSongsRequest request)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Retrieve all files with an extension matching one of the accepted formats.
    /// </summary>
    /// <param name="folderPaths">The folders to look at</param>
    /// <param name="acceptedFormats">The accepted formats</param>
    /// <returns>A collection of filepath as string.</returns>
    /// <since>0.3.0</since>
    private List<string> GetFilesWithSpecifiedFormats(List<string> folderPaths, List<string> acceptedFormats)
    {
        var formatSet = new HashSet<string>(acceptedFormats.Select(f => f.ToLower()));
        return folderPaths
            .SelectMany(folder => Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories))
            .Where(file => formatSet.Contains(Path.GetExtension(file).ToLower()))
            .ToList();
    }
}