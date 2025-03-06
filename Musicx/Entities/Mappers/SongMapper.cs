using log4net;
using MusicxApi.Models;

namespace Musicx.Entities.Mappers;

public static class SongMapper
{
    private static readonly ILog Logger = LogManager.GetLogger(typeof(SongMapper));
    
    // Mappage de l'entité vers le DTO
    public static Song? ToDto(this SongEntity? songEntity)
    {
        if (null == songEntity)
        {
            Logger.Warn("⚠️ Entity is null.");
            return null;
        }
        
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
    public static SongEntity? ToEntity(this Song? songDto)
    {
        if (null == songDto)
        {
            Logger.Warn("⚠️ Entity is null.");
            return null;
        }
        
        var genres = songDto.GenreIds.Select(genreId => new SongGenreEntity
        {
            GenreId = genreId
        }).ToList();

        var influenceGenres = songDto.InfluenceGenreIds.Select(genreId => new SongInfluenceGenreEntity
        {
            GenreId = genreId
        }).ToList();
        
        var songEntity = new SongEntity
        {
            Id = songDto.Id,
            
            Genres = genres,
            InfluenceGenres = influenceGenres,
            
            AlbumId = songDto.AlbumId,
            ArtistId = songDto.ArtistId,
            AudioFormat = songDto.AudioFormat,
            BitRate = songDto.BitRate,
            DiscNumber = songDto.DiscNumber,
            Duration = songDto.Duration,
            Filepath = songDto.Filepath,
            GeneratedGenreName = songDto.GeneratedGenreName,
            Lyrics = songDto.Lyrics,
            SampleRate = songDto.SampleRate,
            Title = songDto.Title,
            TrackNumber = songDto.TrackNumber,
            CreatedAt = songDto.CreatedAt,
            UpdatedAt = songDto.UpdatedAt
        };

        return songEntity;
    }
}
