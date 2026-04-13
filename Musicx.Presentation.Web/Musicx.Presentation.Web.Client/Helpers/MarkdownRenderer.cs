using Markdig;
using Microsoft.AspNetCore.Components;

namespace Musicx.Presentation.Web.Client.Helpers;

public sealed class MarkdownRenderer
{
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();
    
    public static MarkupString Render(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return new MarkupString(string.Empty);
        }

        var html = Markdown.ToHtml(content, Pipeline);
        return new MarkupString(html);
    }
}