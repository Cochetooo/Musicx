using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders;

internal sealed class ArtistSqlBuilder(ILoggerProvider loggerProvider) : SqlBuilder<InArtist>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(ArtistSqlBuilder));
    
    internal override async Task<object?> ExecuteInsert(InArtist entity,
        NpgsqlConnection conn, NpgsqlTransaction? transaction = null)
    {
        var createCommandSql = BuildInsert("artists",
            new Dictionary<string, object?>
            {
                { ArtistColumns.CreatedAt, DateTime.Now },
                { ArtistColumns.UpdatedAt, DateTime.Now },
                { ArtistColumns.Alias, entity.Alias },
                { ArtistColumns.ArtworkUrl, entity.ArtworkUrl },
                { ArtistColumns.CurrentCountry, entity.CurrentCountry },
                { ArtistColumns.CurrentRegion, entity.CurrentRegion },
                { ArtistColumns.CurrentTown, entity.CurrentTown },
                { ArtistColumns.Description, entity.Description },
                { ArtistColumns.Name, entity.Name },
                { ArtistColumns.OriginCountry, entity.OriginCountry },
                { ArtistColumns.OriginRegion, entity.OriginRegion },
                { ArtistColumns.OriginTown, entity.OriginTown },
                { ArtistColumns.Discriminator, entity.Discriminator },
                { ArtistColumns.FormationDate, entity.FormationDate },
                { ArtistColumns.SplitDate, entity.SplitDate },
                { ArtistColumns.FirstName, entity.FirstName },
                { ArtistColumns.LastName, entity.LastName },
                { ArtistColumns.BirthDate, entity.BirthDate },
                { ArtistColumns.DeathDate, entity.DeathDate }
            }, ArtistColumns.Id);
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(createCommandSql.Query, createCommandSql.Parameters));

        await using var cmd = new NpgsqlCommand(createCommandSql.Query, conn, transaction);
        cmd.Parameters.AddRange(createCommandSql.Parameters.ToArray());
        var artistId = (long)(await cmd.ExecuteScalarAsync() ??
                              throw new NullReferenceException("Could not insert entity."));

        return artistId;
    }

    internal override async Task ExecuteUpdate(InArtist entity,
        NpgsqlConnection conn, NpgsqlTransaction? transaction = null)
    {
        var updateCommandSql = BuildUpdate("artists",
            ArtistColumns.Id,
            entity.Id,
            new Dictionary<string, object?>
            {
                { ArtistColumns.UpdatedAt, DateTime.Now },
                { ArtistColumns.Alias, entity.Alias },
                { ArtistColumns.ArtworkUrl, entity.ArtworkUrl },
                { ArtistColumns.CurrentCountry, entity.CurrentCountry },
                { ArtistColumns.CurrentRegion, entity.CurrentRegion },
                { ArtistColumns.CurrentTown, entity.CurrentTown },
                { ArtistColumns.Description, entity.Description },
                { ArtistColumns.Name, entity.Name },
                { ArtistColumns.OriginCountry, entity.OriginCountry },
                { ArtistColumns.OriginRegion, entity.OriginRegion },
                { ArtistColumns.OriginTown, entity.OriginTown },
                { ArtistColumns.Discriminator, entity.Discriminator },
                { ArtistColumns.FormationDate, entity.FormationDate },
                { ArtistColumns.SplitDate, entity.SplitDate },
                { ArtistColumns.FirstName, entity.FirstName },
                { ArtistColumns.LastName, entity.LastName },
                { ArtistColumns.BirthDate, entity.BirthDate },
                { ArtistColumns.DeathDate, entity.DeathDate }
            });
        
        _logger.LogDebug(SqlHelper.InterpolateQuery(updateCommandSql.Query, updateCommandSql.Parameters));

        await using var cmd = new NpgsqlCommand(updateCommandSql.Query, conn, transaction);
        cmd.Parameters.AddRange(updateCommandSql.Parameters.ToArray());
        await cmd.ExecuteScalarAsync();
    }

    internal override string BuildSelect(IQuerySpecification<InArtist>? spec = null, bool distinct = false)
    {
        return distinct 
            ? "SELECT DISTINCT ar0.* FROM artists ar0"
            : "SELECT ar0.* FROM artists ar0";
    }

    internal override string BuildGroupBy(IQuerySpecification<InArtist>? spec = null)
    {
        throw new NotImplementedException();
    }
}