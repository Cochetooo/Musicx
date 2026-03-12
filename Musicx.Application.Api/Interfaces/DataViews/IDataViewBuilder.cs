namespace Musicx.Application.Api.Interfaces.DataViews;

public interface IDataViewBuilder<in TKey, TView>
    where TView : class
{
    Task<TView?> BuildAsync(TKey key, CancellationToken cancellationToken = default);
}