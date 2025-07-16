using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Presentation.Desktop.ViewModels;
using ReactiveUI;

namespace Musicx.Presentation.Desktop.Services;

public interface IContentViewProvider
{
    public ViewModelBase? Content { get; }
    public void SwitchTo(ViewModelBase? content);
}

public sealed class ContentViewProvider(
    ILoggerFactory loggerFactory) : ReactiveObject, IContentViewProvider
{
    private readonly ILogger<ContentViewProvider> _logger = loggerFactory.CreateLogger<ContentViewProvider>();

    private ViewModelBase? _content;
    public ViewModelBase? Content
    {
        get => _content;
        private set => this.RaiseAndSetIfChanged(ref _content, value);
    }

    public void SwitchTo(ViewModelBase? content)
    {
        if (null == content)
        {
            _logger.Warn("⚠️ View model is null.");
            return;
        }

        Content = content;
    }
}