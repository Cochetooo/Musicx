namespace Musicx.Application.Desktop.Models;

public sealed class LibraryProfile
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public IReadOnlyList<string> FolderPaths { get; set; } = [];
    public bool AutoHydrateMetadata { get; set; }
    public bool AutoScanOnStartup { get; set; }
}