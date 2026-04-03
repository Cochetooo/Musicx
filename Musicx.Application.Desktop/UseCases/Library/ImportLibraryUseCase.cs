using System.Collections.Concurrent;
using Musicx.Application.Desktop.Interfaces.Library;
using Musicx.Application.Desktop.Models;

namespace Musicx.Application.Desktop.UseCases.Library;

public interface IImportLibraryUseCase
{
    Task<LibraryImportResult> ExecuteAsync(LibraryProfile profile, CancellationToken cancellationToken = default);
}

public sealed class ImportLibraryUseCase(
    IAudioMetadataReader metadataReader,
    IEncyclopediaMatcher encyclopediaMatcher,
    ILocalLibraryRepository localLibraryRepository,
    IImportProgressPublisher progressPublisher) : IImportLibraryUseCase
{
    private static readonly string[] DefaultFormats =
        [".mp3", ".ogg", ".flac", ".wav", ".m4a", ".aac", ".wma", ".aiff", ".alac"];

    public async Task<LibraryImportResult> ExecuteAsync(LibraryProfile profile, CancellationToken cancellationToken = default)
    {
        var files = profile.FolderPaths
            .Where(Directory.Exists)
            .SelectMany(path => Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories))
            .Where(path => DefaultFormats.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var issues = new ConcurrentBag<ImportIssue>();
        var tracks = new ConcurrentBag<LocalTrack>();
        var processed = 0;
        var imported = 0;
        var failed = 0;

        Publish(files.Length, processed, imported, failed, issues);

        await Parallel.ForEachAsync(files, new ParallelOptions
        {
            MaxDegreeOfParallelism = Math.Max(2, Environment.ProcessorCount / 2),
            CancellationToken = cancellationToken
        }, async (filePath, ct) =>
        {
            try
            {
                var track = await metadataReader.ReadAsync(filePath, ct);
                if (profile.AutoHydrateMetadata)
                {
                    var match = await encyclopediaMatcher.MatchTrackAsync(track, ct);
                    track.ApiSongId = match.ApiSongId;
                    track.ApiAlbumId = match.ApiAlbumId;
                    track.ApiArtistId = match.ApiArtistId;
                    track.Title = match.CanonicalTitle ?? track.Title;
                    track.Artist = match.CanonicalArtist ?? track.Artist;
                    track.Album = match.CanonicalAlbum ?? track.Album;
                    track.IsMatchedWithApi = match.Found;
                }

                tracks.Add(track);
                Interlocked.Increment(ref imported);
            }
            catch (Exception ex)
            {
                issues.Add(new ImportIssue(filePath, ex.Message));
                Interlocked.Increment(ref failed);
            }
            finally
            {
                Interlocked.Increment(ref processed);
                Publish(files.Length, processed, imported, failed, issues);
            }
        });

        await localLibraryRepository.UpsertTracksAsync(tracks.ToArray(), cancellationToken);

        return new LibraryImportResult(files.Length, imported, failed, issues.ToArray());
    }

    private void Publish(int total, int processed, int imported, int failed, ConcurrentBag<ImportIssue> issues)
    {
        progressPublisher.Publish(new ImportProgressSnapshot(total, processed, imported, failed, issues.ToArray()));
    }
}