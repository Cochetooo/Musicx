using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Application.Desktop.Interfaces.UseCases.LocalLibrary;

namespace Musicx.Application.Desktop.UseCases.LocalLibrary;

public class UcPersistLocalSongs(
    ILoggerFactory loggerFactory,
    IBatchImportRepository batchImportRepository) : IPersistLocalSongsUseCase
{
    private readonly ILogger<UcPersistLocalSongs> _logger = loggerFactory.CreateLogger<UcPersistLocalSongs>();
    
    public async Task<PersistLocalSongsResponse> ExecuteAsync(PersistLocalSongsRequest request)
    {
        _logger.Debug("⛏️ Execute : PersistLocalSongs");

        await batchImportRepository.PersistBatchAsync(
            request.Songs,
            request.Albums,
            request.Artists);
        
        _logger.Debug("✅ PersistLocalSongs success!");

        return new PersistLocalSongsResponse();
    }

    public PersistLocalSongsResponse Execute(PersistLocalSongsRequest request)
    {
        throw new NotImplementedException();
    }
}