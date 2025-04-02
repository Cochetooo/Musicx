using Musicx.Application.Common.Interfaces.Common;

namespace Musicx.Application.Desktop.Interfaces.UseCases.LocalLibrary;

public sealed record ImportLocalSongsRequest(
    List<string> FolderPaths,
    List<string> AcceptedFormats,
    IProgressListener ProgressListener) : BaseRequest;

public sealed record ImportLocalSongsResponse(
    int TotalFileCount,
    int SuccessfulFileCount,
    int FailedFileCount) : BaseResponse;

public interface IImportLocalSongsUseCase : IUseCase<ImportLocalSongsRequest, ImportLocalSongsResponse>;