using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Contracts.Dto.Requests.Song;
using Musicx.Contracts.Dto.Requests.Specifics;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Artist;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Dto.Responses.User;

namespace Musicx.Application.Shared.Interfaces.Common;

public interface IApiClient
{
    #region Generics
    
    Task<OutGenericList<TOut>> FindAsync<TIn, TOut>(
        IFindQuery<TIn>? query = null,
        IJoinSpecification<TIn>? joins = null,
        OrderSpecification<TIn>? order = null,
        PagingOptions? pagingOptions = null)
        where TIn : BaseInputModel
        where TOut : BaseOutputModel;

    Task<List<TOut>> FindInAsync<TIn, TOut>(
        IEnumerable<long> ids,
        IJoinSpecification<TIn>? joins = null,
        OrderSpecification<TIn>? order = null)
        where TIn : BaseInputModel
        where TOut : BaseOutputModel;

    Task<TOut?> FindByIdAsync<TIn, TOut>(long id, IJoinSpecification<TIn>? joins = null)
        where TIn : BaseInputModel
        where TOut : BaseOutputModel;

    Task<HttpResponseMessage> SaveAsync<T>(T entity) where T : class;

    Task<HttpResponseMessage> SaveAllAsync<T>(IEnumerable<T> entities) where T : BaseInputModel;

    Task DeleteAsync<T>(long id) where T : class;

    Task DeleteAllAsync<T>(IEnumerable<long> ids) where T : class;

    Task<long> CountAsync<TIn, TOut>(IFindQuery<TIn>? query = null)
        where TIn : BaseInputModel
        where TOut : BaseOutputModel;

    Task<TOut?> GetDataViewAsync<TOut>(string resource, long id, IDictionary<string, object?>? parameters = null)
        where TOut : class;
    
    #endregion
    
    #region Specifics
    
    Task<OutAlbumList> FindAlbumsByArtistAsync(long artistId, IJoinSpecification<InAlbum>? joins = null, OrderSpecification<InAlbum>? order = null);

    Task<OutAlbumList> FindAlbumsByGenreAsync(long genreId, int genreOptions, IJoinSpecification<InAlbum>? joins = null, OrderSpecification<InAlbum>? order = null, PagingOptions? pagingOptions = null);

    Task<OutAlbumList> FindAlbumsByChartAsync(AlbumChartQuery query);

    Task<List<OutSong>> FindSongsByAlbumAsync(long albumId, IJoinSpecification<InSong>? joins = null, OrderSpecification<InSong>? order = null);

    Task<OutUserSongAttribute?> FindSongAttributeByUserAsync(long userId, long songId);
    
    Task<OutGenericList<OutArtist>> FindArtistsByGenreAsync(long genreId, PagingOptions? pagingOptions = null);
    
    Task<OutGenericList<OutUserAlbumAttribute>> FindAlbumReviewsAsync(long albumId, PagingOptions? pagingOptions = null);

    Task<OutGenericList<OutAlbum>> FindSimilarAlbumsAsync(
        long albumId,
        OrderSpecification<InAlbum>? order = null,
        PagingOptions? pagingOptions = null);
    
    #endregion
}