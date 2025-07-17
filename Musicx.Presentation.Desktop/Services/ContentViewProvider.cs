using Microsoft.Extensions.Logging;
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
    ILoggerProvider loggerProvider) : ReactiveObject, IContentViewProvider
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(ContentViewProvider));

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
            _logger.LogWarning("⚠️ View model is null.");
            return;
        }

        Content = content;
    }
}