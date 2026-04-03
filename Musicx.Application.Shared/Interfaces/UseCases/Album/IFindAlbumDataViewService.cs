using Musicx.Contracts.Dto.Responses.Specifics.Albums;

namespace Musicx.Application.Shared.Interfaces.UseCases.Album;

/// <summary>
/// Retrieves the consolidated Album DataView for Web clients.
/// </summary>
/// <since>0.7.4</since>
public interface IFindAlbumDataViewService
{
    /// <summary>
    /// Loads album-centric data optimized for the Album page.
    /// </summary>
    /// <param name="albumId">Target album identifier.</param>
    /// <param name="userId">Optional current user id to personalize user-specific fields.</param>
    Task<OutAlbumDataView?> ExecuteAsync(long albumId, long? userId = null);
}