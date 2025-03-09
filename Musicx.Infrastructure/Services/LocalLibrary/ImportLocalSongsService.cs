using System.Diagnostics;
using log4net;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Core.Models;
using Musicx.Infrastructure.Listeners;
using Musicx.Infrastructure.Managers;
using Musicx.Infrastructure.Repositories;
using Musicx.Infrastructure.Services.Audio;

namespace Musicx.Infrastructure.Services.LocalLibrary;

public sealed class ImportLocalSongsService(
    ReadAudioFileService readAudioFileService,
    ILoggerFactory loggerFactory,
    ISongManager songManager) : IService
{
    private readonly ILogger Logger = loggerFactory.CreateLogger(typeof(ImportLocalSongsService));
    
    public async Task Execute(
        List<string> folderPaths, 
        List<string> acceptedFormats,
        IProgressListener progressListener)
    {
        var benchmark = new Stopwatch();
        benchmark.Start();
        
        // Retrieve audio files from all the library folder paths
        var audioFiles = GetAudioFiles(folderPaths, acceptedFormats);
        var fileNumbers = audioFiles.Count;

        if (0 == fileNumbers)
        {
            Logger.Warn($"⚠️ No audio file found in folders {string.Join(',', folderPaths)}");
            progressListener.UpdateProgress(1, 1);
            return;
        }
        
        progressListener.UpdateProgress(0, fileNumbers);
        var fileProgressCount = 0;
            
        var songs = new List<Song>();
        
        var tasks = audioFiles.Select(async filePath =>
        {
            try
            {
                var song = await readAudioFileService.ExecuteAsync(filePath);
                songs.Add(song);
            }
            catch (Exception ex)
            {
                Logger.Warn($"⚠️ Could not read audio file: {filePath}");
            }
            
            var progress = Interlocked.Increment(ref fileProgressCount);
            progressListener.UpdateProgress(progress, fileNumbers);
        }).ToList();
        
        await Task.WhenAll(tasks);

        await songManager.SaveAll(songs);
        
        progressListener.UpdateProgress(1, 1);
        Logger.Info($"Finished importing {fileProgressCount} audio files in {benchmark.Elapsed.TotalMilliseconds} ms.");
    }
    
    private List<string> GetAudioFiles(List<string> folderPaths, List<string> acceptedFormats)
    {
        // Simule la récupération des fichiers audio
        return folderPaths.SelectMany(folder => Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories)
                .Where(file => acceptedFormats.Contains(Path.GetExtension(file).ToLower())))
            .ToList();
    }
}