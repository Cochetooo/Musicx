using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;


namespace Musicx.Contracts.Mappers;

public static class SongMapper
{
    public static OutSong FromDicoToSong(this IDictionary<string, object?> song) => new()
    {
        /*Id = song.Id,

        CreatedAt = song.CreatedAt,
        UpdatedAt = song.UpdatedAt,

        Album = song.Album?.ToDto(),
        Artist = song.Artist?.ToDto(),
        
        PrimaryGenres = song.PrimaryGenres.Select(g => new OutGenre { Id = g.Id, Name = g.Name }).ToList(),
        InfluenceGenres = song.InfluenceGenres.Select(g => new OutGenre { Id = g.Id, Name = g.Name }).ToList(),

        DiscNumber = song.DiscNumber,
        Duration = song.Duration,
        Lyrics = song.Lyrics,
        Title = song.Title,
        TrackNumber = song.TrackNumber,
        Type = song.Type,

        BitRate = song.BitRate,
        FilePath = song.FilePath,
        Format = song.Format,
        SampleRate = song.SampleRate,
        VolumeModifier = song.VolumeModifier*/
    };

    public static InSong ToRaw(this OutSong song) => new()
    {
        Id = song.Id,
        AlbumId = song.Album?.Id,
        ArtistId = song.Artist?.Id,

        PrimaryGenreIds = song.PrimaryGenres.Select(g => g.Id).ToList(),
        InfluenceGenreIds = song.InfluenceGenres.Select(g => g.Id).ToList(),

        DiscNumber = song.DiscNumber,
        Duration = song.Duration,
        Lyrics = song.Lyrics,
        Title = song.Title,
        TrackNumber = song.TrackNumber,
        Type = song.Type,

        BitRate = song.BitRate,
        FilePath = song.FilePath,
        Format = song.Format,
        SampleRate = song.SampleRate,
        VolumeModifier = song.VolumeModifier
    };
}