using System.Data.Common;
using System.Dynamic;

namespace Musicx.Application.Shared.Interfaces.Persistence;

public interface IDbConnectionProvider
{
    DbConnection CreateConnection();
    Task ExecuteTransactionAsync(params (string sql, IReadOnlyList<DbParameter> parameters)[] commands);
    Task<List<ExpandoObject>> FetchListDynamicAsync(string sql, IReadOnlyList<DbParameter> parameters);
    Task<long> Count(string table);
}