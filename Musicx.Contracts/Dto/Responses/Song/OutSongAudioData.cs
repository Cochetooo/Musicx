using Musicx.Contracts.Enums;

namespace Musicx.Contracts.Dto.Responses;

public sealed class OutSongAudioData : BaseOutputModel
{
    public OutSong Song { get; set; } = null!;
    
    public ushort? BitRate { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public AudioFormatType? Format { get; set; }
    public double? SampleRate { get; set; }
    public double? VolumeModifier { get; set; }
}