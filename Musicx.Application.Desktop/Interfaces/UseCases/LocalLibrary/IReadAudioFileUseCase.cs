using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Contracts.Dto.Requests;


namespace Musicx.Application.Desktop.Interfaces.UseCases.LocalLibrary;

/// <summary>
/// 
/// </summary>
/// <param name="FilePath">Audio file path</param>
/// <param name="AutoCheck">If <b>true</b>, will automatically check in the API if data exists, else creates
/// manually the data.</param>
public sealed record ReadAudioFileRequest(
    string FilePath,
    bool AutoCheck) : BaseRequest;

public sealed record ReadAudioFileResponse(
    InSong Song,
    InAlbum Album,
    InArtist Artist) : BaseResponse;

public interface IReadAudioFileUseCase : IUseCase<ReadAudioFileRequest, ReadAudioFileResponse>;