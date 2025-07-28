using Microsoft.Extensions.Logging;
using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Application.Shared.Interfaces.Common;


namespace Musicx.Infrastructure.Desktop.Persistence.Repositories;

internal sealed class BatchImportRepository(
    ILoggerProvider loggerProvider,
    AppDbContext dbContext) : IBatchImportRepository
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(BatchImportRepository));
    
    public async Task PersistBatchAsync(IEnumerable<Song> songs, IEnumerable<Album> albums, IEnumerable<Artist> artists)
    {
        var songList = songs.ToList();
        var albumList = albums.ToList();
        var artistList = artists.ToList();
        
        _logger.LogDebug($"📄 Batch Import : {songList.Count}");
        
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            dbContext.Songs.AddRange(songList);
            dbContext.Albums.AddRange(albumList);
            dbContext.Artists.AddRange(artistList);

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogCritical("❌ Could not persist batch.", ex);
            await transaction.RollbackAsync();
            throw;
        }
    }
}