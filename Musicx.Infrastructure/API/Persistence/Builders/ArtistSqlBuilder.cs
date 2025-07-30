using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders;

internal sealed class ArtistSqlBuilder : SqlBuilder<InArtist>
{
    internal override (string, List<NpgsqlParameter>) BuildInsert(InArtist entity,
        NpgsqlConnection conn, NpgsqlTransaction? transaction = null)
    {
        throw new NotImplementedException();
    }

    internal override (string, List<NpgsqlParameter>) BuildUpdate(InArtist entity,
        NpgsqlConnection conn, NpgsqlTransaction? transaction = null)
    {
        throw new NotImplementedException();
    }

    internal override string BuildSelect(IQuerySpecification<InArtist>? spec = null)
    {
        throw new NotImplementedException();
    }

    internal override string BuildGroupBy(IQuerySpecification<InArtist>? spec = null)
    {
        throw new NotImplementedException();
    }
}