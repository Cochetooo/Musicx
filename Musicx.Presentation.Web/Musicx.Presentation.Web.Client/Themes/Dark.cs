using Blazorise;

namespace Musicx.Presentation.Web.Client.Themes;

public static class Dark
{
    public static readonly Theme DarkTheme = new Theme
    {
        IsRounded = true,
        BodyOptions = new ThemeBodyOptions
        {
            BackgroundColor = "#2E2E2E",
            TextColor = "#FFFFFF"
        },
        ColorOptions = new ThemeColorOptions
        {
            Primary = "#1F1F1F",
            Secondary = "#2B2B2B",
            Success = "#388E3C",
            Danger = "#D32F2F",
            Warning = "#FFA000",
            Info = "#1976D2",
            Dark = "#141414",
            Light = "#FFFFFF"
        },
        BackgroundOptions = new ThemeBackgroundOptions
        {
            Primary = "#1F1F1F",
            Secondary = "#252525",
            Success = "#1E4623",
            Danger = "#7B1A1A",
            Warning = "#C66900",
            Info = "#0B3861",
            Body = "#2E2E2E",
            Dark = "#1A1A1A",
            Light = "#FFFFFF"
        },
        TextColorOptions = new ThemeTextColorOptions
        {
            Primary = "#FFFFFF",
            Secondary = "#CCCCCC",
            Success = "#388E3C",
            Danger = "#D32F2F",
            Warning = "#FFA000",
            Info = "#1976D2",
            Body = "#F0F0F0",
            Dark = "#0F0F0F",
            Light = "#FFFFFF"
        },
    };
}