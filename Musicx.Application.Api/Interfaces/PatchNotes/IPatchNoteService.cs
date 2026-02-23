namespace Musicx.Application.Api.Interfaces.PatchNotes;

/// <summary>
/// 
/// </summary>
/// <since>0.7.1</since>
public interface IPatchNoteService
{
    Dictionary<string, List<string>> GetVersions();
}