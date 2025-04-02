using Musicx.Application.Common.Interfaces.Common;
using Musicx.Application.Data;
using Musicx.Application.Desktop.Interfaces.UseCases.LocalLibrary;

namespace Musicx.Infrastructure.Desktop.Services.LocalLibrary;

public class UcReadAudioFile(
    ILoggerFactory loggerFactory) : IReadAudioFileUseCase
{
    private readonly ILogger<UcReadAudioFile> _logger = loggerFactory.CreateLogger<UcReadAudioFile>();
    
    public async Task<ReadAudioFileResponse> ExecuteAsync(ReadAudioFileRequest request)
    {
        _logger.Debug("⛏️ Execute : ReadAudioFile");

        _logger.Warn("⚠️ Fake informations, please fix later.");
        var localSong = new LocalSongDto
        {
            SongTitle = "SongTitle"
        };

        _logger.Debug("✅ ReadAudioFile success!");
        return new ReadAudioFileResponse(localSong);
    }

    public ReadAudioFileResponse Execute(ReadAudioFileRequest request)
    {
        throw new NotImplementedException();
    }
}