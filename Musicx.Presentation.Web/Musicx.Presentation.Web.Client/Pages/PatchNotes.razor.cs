using Markdig;

namespace Musicx.Presentation.Web.Client.Pages;

public partial class PatchNotes
{
    private ILogger _logger;
    
    private List<string> _majorVersions = [];
    private List<string> _minorVersions = [];

    private Dictionary<string, List<string>> _versions = [];

    private string? _selectedMajor;
    private string? _selectedMinor;

    private string? _renderedHtml;
    private bool _isLoading;

    protected override async Task OnInitializedAsync()
    {
        _logger = LoggerFactory.CreateLogger(nameof(PatchNotes));

        _versions = await UcPatchNotes.ListPatchNotesAsync();
        
        _majorVersions = _versions.Keys.ToList();
        _selectedMajor = _majorVersions[0];

        if (_selectedMajor is null)
        {
            _logger.LogWarning("⚠️ No major versions found, cannot continue.");
            return;
        }

        _minorVersions = _versions[_selectedMajor];
        await LoadPatch(_minorVersions[0]);
    }

    private async Task OnMajorChanged(string value)
    {
        _selectedMajor = value;
        
        _minorVersions = _versions[_selectedMajor];
        await LoadPatch(_minorVersions[0]);
    }

    private async Task LoadPatch(string version)
    {
        if (_selectedMajor is null)
        {
            _logger.LogWarning("⚠️ Major or minor version is null, cannot load patch.");
            return;
        }
        
        try
        {
            _isLoading = true;
            _selectedMinor = version;

            var markdown = await UcPatchNotes.GetPatchNoteAsync(_selectedMajor, _selectedMinor);

            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();

            _renderedHtml = Markdown.ToHtml(markdown, pipeline);
        }
        finally
        {
            _isLoading = false;
        }
    }
}