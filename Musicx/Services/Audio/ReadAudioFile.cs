using log4net;
using Musicx.Models.Enums;
using MusicxApi.Models;

namespace Musicx.Services.Audio;

public class ReadAudioFile
{
    private static readonly ILog Logger = LogManager.GetLogger(typeof(ReadAudioFile));

    public static void Execute(string filePath, out Song song)
    {
        Logger.Debug("⛏️ Executing ReadAudioFile");
        var file = TagLib.File.Create(filePath);
        
        Logger.Warn("⚠️ Audio Format set to MP3 per default. Fix later");
        ReadSong(file, out song);
        
        Logger.Debug("✅ ReadAudioFile success");
    }

    private static void ReadSong(in TagLib.File file, out Song song)
    {
        Logger.Debug("➕ Reading Song Data");
        song = new Song
        {
            AudioFormat = AudioFormatType.Mp3,
            BitRate = file.Properties.AudioBitrate,
            DiscNumber = file.Tag.Disc,
            Duration = (long) file.Properties.Duration.TotalSeconds,
            Filepath = file.Name,
            SampleRate = file.Properties.AudioSampleRate,
            Title = file.Tag.Title,
            TrackNumber = file.Tag.Track
        };
        
        song.SetRawLyrics(file.Tag.Lyrics);
        
        Logger.Info($"Track : {song.BitRate} Kbps | {song.Duration}s | {song.Filepath} | {song.SampleRate} Hz | {song.Title}");
    }

    private static void ReadAlbum(in TagLib.File file, Song song)
    {
        Logger.Debug("➕ Reading Album Data");
        
        
    }

    private static void ReadArtist(in TagLib.File file, Song song)
    {
        Logger.Debug("➕ Reading Artist Data");
    }

    private static void ReadGenres(in TagLib.File file, Song song)
    {
        Logger.Debug("➕ Reading Genres Data");
    }

    private static void ReadLabel(in TagLib.File file, Song song)
    {
        Logger.Debug("➕ Reading Label Data");
    }
}
