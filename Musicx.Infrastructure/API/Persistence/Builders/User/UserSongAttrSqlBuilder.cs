using Microsoft.Extensions.Logging;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Infrastructure.API.Persistence.Columns.Song;
using Musicx.Infrastructure.API.Persistence.Columns.User;
using Musicx.Infrastructure.Shared.Helpers;
using Npgsql;

namespace Musicx.Infrastructure.API.Persistence.Builders.User;

internal sealed class UserSongAttrSqlBuilder(ILoggerProvider loggerProvider) : SqlBuilder<InUserSongAttribute>
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(UserSongAttrSqlBuilder));

    internal override Task<object?> ExecuteInsert(InUserSongAttribute entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
        => throw new NotImplementedException();

    internal override Task ExecuteUpdate(InUserSongAttribute entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
        => throw new NotImplementedException();

    internal override async Task ExecuteUpsert(InUserSongAttribute entity, NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        var now = DateTime.Now;
        var command = UpsertBuilder.Build(
            table: "user_song_attrs",
            insertProperties: new Dictionary<string, object?>
            {
                { UserSongAttrColumns.UserId, entity.UserId },
                { UserSongAttrColumns.SongId, entity.SongId },
                { UserSongAttrColumns.CreatedAt, now },
                { UserSongAttrColumns.UpdatedAt, now },
                { UserSongAttrColumns.Rating, entity.Rating },
                { UserSongAttrColumns.ProductionRating, entity.ProductionRating },
                { UserSongAttrColumns.LyricsRating, entity.LyricsRating },
                { UserSongAttrColumns.InstrumentationRating, entity.InstrumentationRating },
                { UserSongAttrColumns.VocalsRating, entity.VocalsRating },
                { UserSongAttrColumns.AtmosphereRating, entity.AtmosphereRating },
                { UserSongAttrColumns.OriginalityRating, entity.OriginalityRating }
            },
            conflictColumns: [UserSongAttrColumns.UserId, UserSongAttrColumns.SongId],
            updateProperties: new Dictionary<string, object?>
            {
                { UserSongAttrColumns.UpdatedAt, now },
                { UserSongAttrColumns.Rating, entity.Rating },
                { UserSongAttrColumns.ProductionRating, entity.ProductionRating },
                { UserSongAttrColumns.LyricsRating, entity.LyricsRating },
                { UserSongAttrColumns.InstrumentationRating, entity.InstrumentationRating },
                { UserSongAttrColumns.VocalsRating, entity.VocalsRating },
                { UserSongAttrColumns.AtmosphereRating, entity.AtmosphereRating },
                { UserSongAttrColumns.OriginalityRating, entity.OriginalityRating }
            });

        _logger.LogDebug(SqlHelper.InterpolateQuery(command.Query, command.Parameters));

        await using var cmd = new NpgsqlCommand(command.Query, connection, transaction);
        cmd.Parameters.AddRange(command.Parameters.ToArray());
        await cmd.ExecuteNonQueryAsync();
    }

    internal override string BuildSelect(IJoinSpecification<InUserSongAttribute>? spec = null, bool distinct = false)
        => distinct
            ? "SELECT DISTINCT usr0.*, u0.*, s0.* FROM user_song_attrs usr0 " +
              $"JOIN users u0 ON usr0.{UserSongAttrColumns.UserId} = u0.{UserColumns.Id} " +
              $"JOIN songs s0 ON usr0.{UserSongAttrColumns.SongId} = s0.{SongColumns.Id} "
            : "SELECT usr0.*, u0.*, s0.* FROM user_song_attrs usr0 " +
              $"JOIN users u0 ON usr0.{UserSongAttrColumns.UserId} = u0.{UserColumns.Id} " +
              $"JOIN songs s0 ON usr0.{UserSongAttrColumns.SongId} = s0.{SongColumns.Id} ";

    internal override string BuildGroupBy(IJoinSpecification<InUserSongAttribute>? spec = null)
        => string.Empty;
}