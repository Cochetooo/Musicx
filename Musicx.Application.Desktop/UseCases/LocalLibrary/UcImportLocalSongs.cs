using System.Collections.Concurrent;
using Musicx.Application.Common.Interfaces.Common;
using Musicx.Application.Desktop.Interfaces.UseCases.LocalLibrary;

namespace Musicx.Application.Desktop.UseCases.LocalLibrary;

/// <summary>
/// Import all local songs from one or many folders.
/// </summary>
/// <since>0.3.0</since>
public sealed class UcImportLocalSongs(
    IReadAudioFileUseCase readAudioFile,
    IPersistLocalSongsUseCase persistLocalSongs,
    ILoggerFactory loggerFactory
    ) : IImportLocalSongsUseCase
{
    private readonly ILogger<UcImportLocalSongs> _logger = loggerFactory.CreateLogger<UcImportLocalSongs>();
    
    /// <summary>
    /// Retrieve audio files from all the library folder paths, then import each files found
    /// into database with multithreaded tasks.
    /// </summary>
    /// <since>0.3.0</since>
    public async Task<ImportLocalSongsResponse> ExecuteAsync(ImportLocalSongsRequest request)
    {
        _logger.Debug("⛏️ Execute : ImportLocalSongs");
        
        // Retrieve audio files from all the library folder paths.
        var audioFiles = GetFilesWithSpecifiedFormats(request.FolderPaths, request.AcceptedFormats);
        var fileNumbers = audioFiles.Count;

        // If there is no file found, no need to pursue the operation.
        if (0 == fileNumbers)
        {
            _logger.Warn($"⚠️ No audio file found in folders {string.Join(',', request.FolderPaths)}");
            request.ProgressListener.UpdateProgress(1, 1, null);
            return new ImportLocalSongsResponse(0, 0, 0);
        }
        
        // Initialize counters
        request.ProgressListener.UpdateProgress(0, fileNumbers, null);
        var fileProgressCount = 0;
        var successfulFileCount = 0;
        var failedFileCount = 0;
        
        var songs = new ConcurrentBag<LocalSongDto>();

        // Create multithreaded tasks
        var tasks = audioFiles.Select(async filePath =>
        {
            try
            {
                var readAudioFileResponse = await readAudioFile.ExecuteAsync(
                    new ReadAudioFileRequest(filePath)
                );
                
                songs.Add(readAudioFileResponse.LocalSong);
                Interlocked.Increment(ref successfulFileCount);
            }
            catch (Exception)
            {
                _logger.Warn($"⚠️ Could not read audio file: {filePath}");
                Interlocked.Increment(ref failedFileCount);
            }
            
            Interlocked.Increment(ref fileProgressCount);
            request.ProgressListener.UpdateProgress(fileProgressCount, fileNumbers, null);
        }).ToList();
        
        await Task.WhenAll(tasks);

        await persistLocalSongs.ExecuteAsync(new PersistLocalSongsRequest(songs));
        
        request.ProgressListener.UpdateProgress(0, 0, null);
        _logger.Debug("✅ ImportLocalSongs success!");

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