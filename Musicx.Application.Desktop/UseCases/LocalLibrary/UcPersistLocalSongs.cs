using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Application.Desktop.Interfaces.UseCases.LocalLibrary;

namespace Musicx.Application.Desktop.UseCases.LocalLibrary;

public class UcPersistLocalSongs(
    ILoggerProvider loggerProvider,
    IBatchImportRepository batchImportRepository) : IPersistLocalSongsClientService
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UcPersistLocalSongs));
    
    public async Task<PersistLocalSongsResponse> ExecuteAsync(PersistLocalSongsRequest request)
    {
        _logger.LogDebug("⛏️ Execute : PersistLocalSongs");

        await batchImportRepository.PersistBatchAsync(
            request.Songs,
            request.Albums,
            request.Artists);
        
        _logger.LogDebug("✅ PersistLocalSongs success!");

        return new PersistLocalSongsResponse();
    }

    public PersistLocalSongsResponse Execute(PersistLocalSongsRequest request)
    {
        throw new NotImplementedException();
    }
}