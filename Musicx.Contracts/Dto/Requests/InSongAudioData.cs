using Musicx.Contracts.Enums;

namespace Musicx.Contracts.Dto.Requests;

public sealed class InSongAudioData : BaseInputModel
{
    public long SongId { get; set; }
    
    public ushort? BitRate { get; set; }
    public string FilePath { get; set; }
    public AudioFormatType? Format { get; set; }
    public double? SampleRate { get; set; }
    public double? VolumeModifier { get; set; }
}