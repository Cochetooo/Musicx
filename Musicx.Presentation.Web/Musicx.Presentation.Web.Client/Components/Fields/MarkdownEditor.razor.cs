using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace Musicx.Presentation.Web.Client.Components.Fields;

public partial class MarkdownEditor
{
    [Parameter] public string Label { get; set; } = "Markdown";
    [Parameter] public int Lines { get; set; } = 6;
    [Parameter] public string? Value { get; set; }
    [Parameter] public EventCallback<string?> ValueChanged { get; set; }

    private MudColor _selectedColor = new("#7E57C2");

    private async Task OnValueChanged(string? value)
    {
        Value = value;
        await ValueChanged.InvokeAsync(value);
    }

    private async Task Insert(string snippet)
    {
        var prefix = string.IsNullOrWhiteSpace(Value) ? string.Empty : "\n";
        Value = $"{Value}{prefix}{snippet}";
        await ValueChanged.InvokeAsync(Value);
    }

    private Task OnColorChanged(MudColor color)
    {
        _selectedColor = color;
        return Task.CompletedTask;
    }

    private Task InsertBold() => Insert("**gras**");
    private Task InsertItalic() => Insert("*italique*");
    private Task InsertHeading() => Insert("## Titre");
    private Task InsertParagraph() => Insert("Paragraphe");
    private Task InsertList() => Insert("- élément 1\n- élément 2");
    private Task InsertDivider() => Insert("---");

    private Task InsertColorSpan()
        => Insert($"<span style=\"color:{_selectedColor.Value};\">texte coloré</span>");
}