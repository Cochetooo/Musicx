namespace Musicx.Application.Web.Interfaces.UseCases.PatchNotes;

public interface IPatchNotesService
{
    Task<Dictionary<string, List<string>>> ListPatchNotesAsync();
    Task<string> GetPatchNoteAsync(string majorVersion, string minorVersion);
}