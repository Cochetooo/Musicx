using System.Text.Json;
using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.UseCases.PatchNotes;

namespace Musicx.Infrastructure.Shared.UseCases.PatchNotes;

public sealed class PatchNotesService(
    HttpClient httpClient,
    ILoggerFactory loggerFactory) : IPatchNotesService
{
    private readonly ILogger _logger = loggerFactory.CreateLogger(nameof(PatchNotesService));

    public async Task<Dictionary<string, List<string>>> ListPatchNotesAsync()
    {
        var url = $"/api/patch-notes";
        
        _logger.LogInformation($"🌍🏳️ LIST PATCH NOTES : {url}");

        try
        {
            var response = await httpClient.GetStringAsync(url);
            var json = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(response);

            if (json is not null)
            {
                _logger.LogInformation($"🌍✅ LIST PATCH NOTES {url} - SUCCESS");
                return json;
            }

            _logger.LogWarning($"🌍⚠️ LIST PATCH NOTES {url} - WARNING NOT FOUND");
        }
        catch (Exception ex)
        {
            _logger.LogError($"🌍❌ LIST PATCH NOTES {url} - ERROR : {ex.Message}");
        }

        return [];
    }

    public async Task<string> GetPatchNoteAsync(string majorVersion, string minorVersion)
    {
        var url = $"/PatchNotes/{majorVersion}/{minorVersion}.md";
        
        _logger.LogInformation($"🌍🏳️ GET PATCH NOTE : {url}");

        try
        {
            var response = await httpClient.GetStringAsync(url);
            
            _logger.LogInformation($"🌍✅ GET PATCH NOTE {url} - SUCCESS");

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"🌍❌ GET PATCH NOTE {url} - ERROR : {ex.Message}");
            return string.Empty;
        }
    }
}