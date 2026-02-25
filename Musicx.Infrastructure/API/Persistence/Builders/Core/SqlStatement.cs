using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders.Core;

/// <summary>
/// Represents a fully built SQL statement along with
/// its associated Npgsql parameters.
/// 
/// This object is returned by builder methods before execution.
/// </summary>
internal record SqlStatement (
    string Query,
    List<NpgsqlParameter> Parameters
);