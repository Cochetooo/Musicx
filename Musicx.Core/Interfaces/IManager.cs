using System.Linq.Expressions;

namespace Musicx.Core.Interfaces;

public interface IManager<T> where T : class
{
    Task Save(T model);
    Task SaveAll(IList<T> models);

    Task Delete(ulong id);

    Task<T?> FindById(ulong id);
    Task<IEnumerable<T>> FindAll(int skip = 0, int take = 100, Expression<Func<T, bool>>? filter = null);
}