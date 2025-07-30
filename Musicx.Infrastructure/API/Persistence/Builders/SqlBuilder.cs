using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders;

internal abstract class SqlBuilder<T> where T : BaseInputModel
{
    internal abstract (string, List<NpgsqlParameter>) BuildInsert(T entity, 
        NpgsqlConnection connection, NpgsqlTransaction? transaction = null);
    
    internal abstract (string, List<NpgsqlParameter>) BuildUpdate(T entity, 
        NpgsqlConnection connection, NpgsqlTransaction? transaction = null);
    
    internal abstract string BuildSelect(IQuerySpecification<T>? spec = null);
    
    internal abstract string BuildGroupBy(IQuerySpecification<T>? spec = null);
}