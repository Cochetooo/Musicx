using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Musicx.Application.Desktop.Enums;
using Musicx.Application.Desktop.Models;
using Musicx.Application.Desktop.UseCases.Library;
using Musicx.Application.Shared.Interfaces.Localization;
using Musicx.Infrastructure.Desktop.Services.Library;
using ReactiveUI;

namespace Musicx.Presentation.Desktop.ViewModels.Pages;

public sealed class LibraryPageViewModel : ViewModelBase
{
    private readonly IGetOrCreateDefaultProfileUseCase _getOrCreateProfileUseCase;
    private readonly IImportLibraryUseCase _importLibraryUseCase;
    private readonly IGetLibrarySnapshotUseCase _getLibrarySnapshotUseCase;
    private readonly ITranslationService _translationService;

    private LibraryProfile? _currentProfile;
    private ImportProgressSnapshot _progress = new(0, 0, 0, 0, []);
    private LibraryDisplayMode _displayMode = LibraryDisplayMode.AlbumGrid;

    public LibraryPageViewModel(
        IGetOrCreateDefaultProfileUseCase getOrCreateProfileUseCase,
        IImportLibraryUseCase importLibraryUseCase,
        IGetLibrarySnapshotUseCase getLibrarySnapshotUseCase,
        ITranslationService translationService,
        InMemoryImportProgressPublisher progressPublisher)
    {
        _getOrCreateProfileUseCase = getOrCreateProfileUseCase;
        _importLibraryUseCase = importLibraryUseCase;
        _getLibrarySnapshotUseCase = getLibrarySnapshotUseCase;
        _translationService = translationService;

        Artists = new ObservableCollection<string>();
        Tracks = new ObservableCollection<LocalTrack>();
        FailedImports = new ObservableCollection<ImportIssue>();

        DisplayModes = Enum.GetValues<LibraryDisplayMode>();

        ImportCommand = ReactiveCommand.CreateFromTask(ImportAsync);
        RefreshCommand = ReactiveCommand.CreateFromTask(RefreshAsync);

        progressPublisher.SnapshotPublished += OnSnapshot;
    }

    public Array DisplayModes { get; }
    public string LibraryProfileLabel => Tr("Desktop.Library.Profile", "Library profile");
    public string ScanNowLabel => Tr("Desktop.Library.ScanNow", "Scan now");
    public string RefreshLabel => Tr("Desktop.Common.Refresh", "Refresh");
    public string ImportProgressLabel => Tr("Desktop.Library.ImportProgress", "Import progress");

    public ObservableCollection<string> Artists { get; }
    public ObservableCollection<LocalTrack> Tracks { get; }
    public ObservableCollection<ImportIssue> FailedImports { get; }

    public LibraryDisplayMode DisplayMode
    {
        get => _displayMode;
        set => this.RaiseAndSetIfChanged(ref _displayMode, value);
    }

    public string ProfileName => _currentProfile?.Name ?? "No profile";
    public string FolderSummary => _currentProfile is null ? "" : string.Join(" ; ", _currentProfile.FolderPaths);

    public int TotalFiles => _progress.TotalFiles;
    public int ProcessedFiles => _progress.ProcessedFiles;
    public int ImportedFiles => _progress.ImportedFiles;
    public int FailedFiles => _progress.FailedFiles;
    public double ProgressRatio => _progress.TotalFiles == 0 ? 0 : (double)_progress.ProcessedFiles / _progress.TotalFiles;

    public ReactiveCommand<Unit, Unit> ImportCommand { get; }
    public ReactiveCommand<Unit, Unit> RefreshCommand { get; }

    public async Task InitializeAsync()
    {
        _currentProfile = await _getOrCreateProfileUseCase.ExecuteAsync();
        this.RaisePropertyChanged(nameof(ProfileName));
        this.RaisePropertyChanged(nameof(FolderSummary));
        await RefreshAsync();

        if (_currentProfile.AutoScanOnStartup)
        {
            await ImportAsync();
        }
    }

    private async Task ImportAsync()
    {
        if (_currentProfile is null)
        {
            return;
        }

        FailedImports.Clear();
        await _importLibraryUseCase.ExecuteAsync(_currentProfile);
        await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        var snapshot = await _getLibrarySnapshotUseCase.ExecuteAsync();

        Artists.Clear();
        foreach (var artist in snapshot.Artists)
        {
            Artists.Add(artist);
        }

        Tracks.Clear();
        foreach (var track in snapshot.Tracks)
        {
            Tracks.Add(track);
        }
    }

    private void OnSnapshot(ImportProgressSnapshot snapshot)
    {
        _progress = snapshot;

        FailedImports.Clear();
        foreach (var issue in snapshot.Issues.Take(25))
        {
            FailedImports.Add(issue);
        }

        this.RaisePropertyChanged(nameof(TotalFiles));
        this.RaisePropertyChanged(nameof(ProcessedFiles));
        this.RaisePropertyChanged(nameof(ImportedFiles));
        this.RaisePropertyChanged(nameof(FailedFiles));
        this.RaisePropertyChanged(nameof(ProgressRatio));
    }

    private string Tr(string key, string fallback)
    {
        var value = _translationService[key];
        return value.StartsWith("!") ? fallback : value;
    }
}