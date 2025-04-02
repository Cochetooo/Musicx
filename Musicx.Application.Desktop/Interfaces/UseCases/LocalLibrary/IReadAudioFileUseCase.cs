using Musicx.Application.Common.Interfaces.Common;

namespace Musicx.Application.Desktop.Interfaces.UseCases.LocalLibrary;

public sealed record ReadAudioFileRequest(
    string FilePath) : BaseRequest;

public sealed record ReadAudioFileResponse(
    LocalSongDto LocalSong) : BaseResponse;

public interface IReadAudioFileUseCase : IUseCase<ReadAudioFileRequest, ReadAudioFileResponse>;