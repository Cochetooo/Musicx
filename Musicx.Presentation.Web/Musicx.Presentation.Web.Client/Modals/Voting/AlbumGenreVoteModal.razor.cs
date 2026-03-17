using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Application.Shared.Enums;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Genre;
using Musicx.Contracts.Dto.Responses.Specifics.Genres;
using Musicx.Contracts.Enums;

namespace Musicx.Presentation.Web.Client.Modals.Voting;

public partial class AlbumGenreVoteModal
{
    private ILogger _logger = null!;
    
    [Parameter] public EventCallback OnSave { get; set; }

    private enum VoteDirection
    {
        Positive,
        Negative
    }
    
    private readonly record struct Section(string Key, string Title, string CssClass);
    private readonly record struct SectionLink(string CssClass);
    
    private sealed class GenreVoteUi
    {
        public OutGenre Genre { get; init; } = null!;
        public VoteDirection UserVote { get; set; } = VoteDirection.Positive;
        public bool IsExpanded { get; set; }
        public List<string> PositiveUsers { get; set; } = [];
        public List<string> NegativeUsers { get; set; } = [];
        public int PositiveCount => PositiveUsers.Count;
        public int NegativeCount => NegativeUsers.Count;
    }

    private readonly List<Section> _sections =
    [
        new("main", "Main Genres", "zone-main"),
        new("primary", "Primary Genres", "zone-primary"),
        new("descriptor", "Descriptors", "zone-descriptor"),
        new("scene", "Scenes", "zone-scene"),
        new("movement", "Movements", "zone-movement"),
        new("influence", "Influences", "zone-influence")
    ];

    private readonly List<SectionLink> _links =
    [
        new("l-main-primary"),
        new("l-primary-descriptor"),
        new("l-descriptor-movement"),
        new("l-movement-influence"),
        new("l-influence-scene"),
        new("l-scene-main")
    ];
    
    private readonly Dictionary<string, GenreType[]> _allowedTypes = new()
    {
        ["main"] = [GenreType.Genre],
        ["primary"] = [GenreType.Subgenre, GenreType.Fusion, GenreType.Localization],
        ["descriptor"] = [GenreType.Descriptor],
        ["scene"] = [GenreType.Scene],
        ["movement"] = [GenreType.Movement],
        ["influence"] = [GenreType.Subgenre, GenreType.Fusion, GenreType.Localization]
    };

    private readonly Dictionary<string, List<GenreVoteUi>> _votesBySection = [];
    private readonly Dictionary<string, OutGenre?> _selectedGenreBySection = [];
    private readonly Dictionary<string, string> _selectedTextBySection = [];

    private List<OutGenre> _availableGenres = [];
    private long _albumId;
    private long? _userId;

    private MudDialog _modalRef = null!;

    protected override async Task OnInitializedAsync()
    {
        _logger = LoggerFactory.CreateLogger(nameof(AlbumGenreVoteModal));
        
        _availableGenres = await UcListGenres.ExecuteAsync(pagingOptions: new PagingOptions(100_000, 0));
        
        foreach (var section in _sections)
        {
            _votesBySection[section.Key] = [];
            _selectedGenreBySection[section.Key] = null;
            _selectedTextBySection[section.Key] = string.Empty;
        }
    }

    public async Task Show(OutAlbum album)
    {
        _albumId = album.Id;
        _userId = UserClientContext.CurrentUser?.Id;

        ResetSelections();
        BuildVotesFromAlbum(album);
        
        await _modalRef.ShowAsync();
        await InvokeAsync(StateHasChanged);
    }

    private void BuildVotesFromAlbum(OutAlbum album)
    {
        foreach (var section in _sections)
        {
            _votesBySection[section.Key].Clear();
        }
        
        var primaryNodes = album.PrimaryGenres ?? [];
        var influenceNodes = album.InfluenceGenres ?? [];
        
        PopulateFromPrimaryNodes(primaryNodes);
        
        PopulateFromInfluenceNodes(influenceNodes);
    }

    private void PopulateFromPrimaryNodes(IEnumerable<AlbumGenreNode> nodes)
    {
        foreach (var group in nodes.GroupBy(x => x.Genre.Id))
        {
            var first = group.First();
            var section = SectionForGenre(first.Genre, false);
            if (section is null)
            {
                continue;
            }
            
            var positiveUsers = group.Where(x => x.Relation.Confidence >= 0f)
                .Select(x => "User")
                .Distinct()
                .OrderBy(x => x)
                .ToList();
            
            var negativeUsers = group.Where(x => x.Relation.Confidence < 0f)
                .Select(x => "User")
                .Distinct()
                .OrderBy(x => x)
                .ToList();
            
            var currentUserVote = 1f;
            //var currentUserVote = group.FirstOrDefault(x => x.Relation.Tagger.Id == _userId)?.Relation.Confidence ?? 0.8f;
            
            _votesBySection[section].Add(new GenreVoteUi
            {
                Genre = first.Genre,
                UserVote = currentUserVote < 0f ? VoteDirection.Negative : VoteDirection.Positive,
                PositiveUsers = positiveUsers,
                NegativeUsers = negativeUsers
            });
        }
    }

    private void PopulateFromInfluenceNodes(IEnumerable<AlbumInfluenceNode> nodes)
    {
        foreach (var group in nodes.GroupBy(x => x.Genre.Id))
        {
            var first = group.First();
            var section = SectionForGenre(first.Genre, true);
            if (section is null)
            {
                continue;
            }
            
            var positiveUsers = group.Where(x => x.Relation.Confidence >= 0f)
                .Select(x => "User")
                .Distinct()
                .OrderBy(x => x)
                .ToList();
            
            var negativeUsers = group.Where(x => x.Relation.Confidence < 0f)
                .Select(x => "User")
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var currentUserVote = 1f;
            //var currentUserVote = group.FirstOrDefault(x => x.Relation.Tagger.Id == _userId)?.Relation.Confidence ?? 0.8f;

            _votesBySection[section].Add(new GenreVoteUi
            {
                Genre = first.Genre,
                UserVote = currentUserVote < 0f ? VoteDirection.Negative : VoteDirection.Positive,
                PositiveUsers = positiveUsers,
                NegativeUsers = negativeUsers
            });
        }
    }

    private string? SectionForGenre(OutGenre genre, bool fromInfluence)
    {
        if (fromInfluence)
        {
            return _allowedTypes["influence"].Contains(genre.Type) ? "influence" : null;
        }
        
        return genre.Type switch
        {
            GenreType.Genre => "main",
            GenreType.Subgenre or GenreType.Fusion or GenreType.Localization => "primary",
            GenreType.Scene => "scene",
            GenreType.Movement => "movement",
            GenreType.Descriptor => "descriptor",
            _ => null
        };
    }
    
    private IReadOnlyList<GenreVoteUi> GetVotes(string section)
        => _votesBySection[section]
            .OrderByDescending(v => v.PositiveCount - v.NegativeCount)
            .ThenBy(v => GenreLabel(v.Genre))
            .ToList();

    private void AddGenre(string section)
    {
        var genre = GetSelectedGenre(section);
        if (genre is null)
        {
            return;
        }

        if (_votesBySection[section].Any(v => v.Genre.Id == genre.Id))
        {
            return;
        }

        _votesBySection[section].Add(new GenreVoteUi
        {
            Genre = genre,
            UserVote = VoteDirection.Positive,
            PositiveUsers = UserClientContext.CurrentUser is null ? [] : [UserClientContext.CurrentUser.Name],
            NegativeUsers = []
        });
        
        SetSelectedGenre(section, null);
        SetSelectedText(section, string.Empty);
    }

    private void Vote(string section, OutGenre genre, VoteDirection direction)
    {
        var vote = _votesBySection[section].FirstOrDefault(v => v.Genre.Id == genre.Id);
        if (vote is null)
        {
            return;
        }
        
        vote.UserVote = direction;

        if (UserClientContext.CurrentUser is not null)
        {
            var userName = UserClientContext.CurrentUser.Name;
            vote.PositiveUsers.Remove(userName);
            vote.NegativeUsers.Remove(userName);

            if (direction == VoteDirection.Positive)
            {
                vote.PositiveUsers.Add(userName);
                vote.PositiveUsers = vote.PositiveUsers.Distinct().OrderBy(x => x).ToList();
            }
            else
            {
                vote.NegativeUsers.Add(userName);
                vote.NegativeUsers = vote.NegativeUsers.Distinct().OrderBy(x => x).ToList();
            }
        }
    }
    
    private void RemoveVote(string section, long genreId)
    {
        _votesBySection[section].RemoveAll(v => v.Genre.Id == genreId);
    }

    private void ToggleExpanded(string section, long genreId)
    {
        var target = _votesBySection[section].FirstOrDefault(v => v.Genre.Id == genreId);
        if (target is null)
        {
            return;
        }
        
        target.IsExpanded = !target.IsExpanded;
    }

    private async Task Save()
    {
        _logger.LogInformation("💾 Saving user genres...");

        if (UserClientContext.CurrentUser is null)
        {
            _logger.LogWarning("⚠️ User must be connected to vote.");
            Snackbar.Add("You must be connected to vote for genres.", Severity.Warning);
            return;
        }
        
        var primaryVotes = _votesBySection.Where(x => x.Key != "influence")
            .SelectMany(x => x.Value)
            .Distinct()
            .Select(v => new InAlbumGenre
            {
                AlbumId = _albumId,
                TaggerId = UserClientContext.CurrentUser.Id,
                GenreId = v.Genre.Id,
                Source = GenreVoteSource.User,
                Confidence = v.UserVote == VoteDirection.Positive ? 1f : -1f
            })
            .ToList();

        var influenceVotes = _votesBySection["influence"]
            .Distinct()
            .Select(v => new InAlbumInfluence
            {
                AlbumId = _albumId,
                TaggerId = UserClientContext.CurrentUser.Id,
                GenreId = v.Genre.Id,
                Source = GenreVoteSource.User,
                Confidence = v.UserVote == VoteDirection.Positive ? 1f : -1f
            })
            .ToList();

        await UcVoteAlbumGenre.ExecuteAsync(primaryVotes);
        await UcVoteAlbumInfluence.ExecuteAsync(influenceVotes);

        Snackbar.Add("Votes has been saved.", Severity.Success);

        await OnSave.InvokeAsync();
        await Hide();
    }

    private async Task Hide()
    {
        await _modalRef.CloseAsync();
    }
    
    private void ResetSelections()
    {
        foreach (var section in _sections)
        {
            _selectedGenreBySection[section.Key] = null;
            _selectedTextBySection[section.Key] = string.Empty;
        }
    }
    
    private OutGenre? GetSelectedGenre(string section) => _selectedGenreBySection[section];
    private void SetSelectedGenre(string section, OutGenre? genre) => _selectedGenreBySection[section] = genre;

    private string GetSelectedText(string section) => _selectedTextBySection[section];
    private void SetSelectedText(string section, string text) => _selectedTextBySection[section] = text;

    private string GenreLabel(OutGenre genre)
        => genre.ShortName ?? genre.CanonicalName;

    private async Task<IEnumerable<OutGenre>> SearchBySection(string section, string? value, CancellationToken token)
    {
        await Task.Delay(10, token);
        
        if (string.IsNullOrWhiteSpace(value))
        {
            return [];
        }
        
        var selectedIds = _votesBySection.Values.SelectMany(x => x).Select(x => x.Genre.Id).ToHashSet();

        return _availableGenres.Where(x =>
            _allowedTypes[section].Contains(x.Type)
            && x is { IsTaggable: true, IsVisible: true }
            && !selectedIds.Contains(x.Id)
            && (x.CanonicalName.Contains(value, StringComparison.InvariantCultureIgnoreCase)
                || (x.ShortName?.Contains(value, StringComparison.InvariantCultureIgnoreCase) ?? false))
        );
    }
}