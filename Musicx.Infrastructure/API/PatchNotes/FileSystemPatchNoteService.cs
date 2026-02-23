using Microsoft.Extensions.Logging;
using Musicx.Application.Api.Interfaces.PatchNotes;
using Musicx.Application.Api.Interfaces.Storage;

namespace Musicx.Infrastructure.API.PatchNotes;

public sealed class FileSystemPatchNoteService(
    IPublicFileRoot fileRoot,
    ILoggerProvider loggerProvider) : IPatchNoteService
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(FileSystemPatchNoteService));
    private readonly string _basePath = Path.Combine(fileRoot.GetWebRootPath(), "PatchNotes");
    
    public Dictionary<string, List<string>> GetVersions()
    {
        var result = new Dictionary<string, List<string>>();

        try
        {
            foreach (var majorDir in Directory.GetDirectories(_basePath))
            {
                var major = Path.GetFileName(majorDir);

                var versions = Directory
                    .GetFiles(majorDir, "*.md")
                    .Select(Path.GetFileNameWithoutExtension)
                    .OrderByDescending(v => v)
                    .ToList();

                result[major] = versions!;
            }

            _logger.LogDebug($"ℹ️ Patch Notes major versions found: {result.Count}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error while counting patch notes version : " + ex.Message + "\n" + ex.StackTrace);
        }

        return result;
    }
}