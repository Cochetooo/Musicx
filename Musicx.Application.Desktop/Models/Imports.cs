namespace Musicx.Application.Desktop.Models;

public sealed record ImportIssue(string FilePath, string Reason);

public sealed record ImportProgressSnapshot(
    int TotalFiles,
    int ProcessedFiles,
    int ImportedFiles,
    int FailedFiles,
    IReadOnlyList<ImportIssue> Issues);
    
public sealed record LibraryImportResult(
    int TotalFiles,
    int ImportedFiles,
    int FailedFiles,
    IReadOnlyList<ImportIssue> Issues);