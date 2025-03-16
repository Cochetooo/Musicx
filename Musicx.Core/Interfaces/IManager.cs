using System.Linq.Expressions;

namespace Musicx.Core.Interfaces;

public interface IManager<T> where T : class
{
    Task<ulong> Save(T model);
    Task<List<ulong>> SaveAll(IList<T> models);

    Task Delete(ulong id);

    Task<uint> GetCount();

    Task<T?> FindById(ulong id);
    Task<List<T>> FindAll(int skip = 0, int take = 100, Expression<Func<T, bool>>? filter = null);
    Task<T?> FindExisting(T t);
    Task<List<T>> FindIn(List<ulong> ids);
}