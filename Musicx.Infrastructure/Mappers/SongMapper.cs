using log4net;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Core.Models;
using Musicx.Data.Entities;
using Musicx.Infrastructure.Loaders;
using Musicx.Infrastructure.Managers;
using Musicx.Infrastructure.Repositories;

namespace Musicx.Infrastructure.Mappers;

public interface ISongMapper : IMapper<Song, SongEntity>;

public class SongMapper(ILoggerFactory loggerFactory) : ISongMapper
{
    private readonly ILogger<SongMapper> Logger = loggerFactory.CreateLogger<SongMapper>();

    // Mappage de l'entité vers le DTO
    public Song ToDto(SongEntity songEntity)
    {
        // Mappage des genres et des genres d'influence
        var genres = songEntity.Genres.Select(g => g.GenreId).ToList();
        var influenceGenres = songEntity.InfluenceGenres.Select(g => g.GenreId).ToList();
        
        var songDto = new Song
        {
            Id = songEntity.Id,
            
            GenreIds = genres,
            InfluenceGenreIds = influenceGenres,
            
            AlbumId = songEntity.AlbumId,
            ArtistId = songEntity.ArtistId,
            AudioFormat = songEntity.AudioFormat,
            BitRate = songEntity.BitRate,
            DiscNumber = songEntity.DiscNumber,
            Duration = songEntity.Duration,
            Filepath = songEntity.Filepath,
            GeneratedGenreName = songEntity.GeneratedGenreName,
            Lyrics = songEntity.Lyrics,
            SampleRate = songEntity.SampleRate,
            Title = songEntity.Title,
            TrackNumber = songEntity.TrackNumber,
            CreatedAt = songEntity.CreatedAt,
            UpdatedAt = songEntity.UpdatedAt
        };

        return songDto;
    }

    // Mappage du DTO vers l'entité
    public SongEntity ToEntity(Song songDto)
    {
        var entity = new SongEntity();
        
        var genres = songDto.GenreIds.Select(genreId => new SongGenreEntity
        {
            GenreId = genreId
        }).ToList();

        var influenceGenres = songDto.InfluenceGenreIds.Select(genreId => new SongInfluenceGenreEntity
        {
            GenreId = genreId
        }).ToList();

        entity.Id = songDto.Id;

        entity.Genres = genres;
        entity.InfluenceGenres = influenceGenres;
        
        entity.AlbumId = songDto.AlbumId;
        entity.ArtistId = songDto.ArtistId;
        entity.AudioFormat = songDto.AudioFormat;
        entity.BitRate = songDto.BitRate;
        entity.DiscNumber = songDto.DiscNumber;
        entity.Duration = songDto.Duration;
        entity.Filepath = songDto.Filepath;
        entity.GeneratedGenreName = songDto.GeneratedGenreName;
        entity.Lyrics = songDto.Lyrics;
        entity.SampleRate = songDto.SampleRate;
        entity.Title = songDto.Title;
        entity.TrackNumber = songDto.TrackNumber;
        entity.CreatedAt = songDto.CreatedAt;
        entity.UpdatedAt = songDto.UpdatedAt;

        return entity;
    }
}
