using Blazorise;

namespace Musicx.Presentation.Web.Client.Themes;

public static class Light
{
    public static readonly Theme LightTheme = new Theme
    {
        IsRounded = true,
        BodyOptions = new ThemeBodyOptions
        {
            BackgroundColor = "#F8F9FA",
            TextColor = "#212529"
        },
        ColorOptions = new ThemeColorOptions
        {
            Primary = "#FFFFFF",
            Secondary = "#E9ECEF",
            Success = "#388E3C",
            Danger = "#D32F2F",
            Warning = "#FFA000",
            Info = "#1976D2",
            Dark = "#343A40",
            Light = "#FFFFFF"
        },
        BackgroundOptions = new ThemeBackgroundOptions
        {
            Primary = "#FFFFFF",
            Secondary = "#F8F9FA",
            Success = "#1E4623",
            Danger = "#7B1A1A",
            Warning = "#C66900",
            Info = "#0B3861",
            Body = "#F8F9FA",
            Dark = "#E9ECEF",
            Light = "#FFFFFF"
        },
        TextColorOptions = new ThemeTextColorOptions
        {
            Primary = "#212529",
            Secondary = "#6C757D",
            Success = "#388E3C",
            Danger = "#D32F2F",
            Warning = "#FFA000",
            Info = "#1976D2",
            Body = "#212529",
            Dark = "#000000",
            Light = "#FFFFFF"
        }
    };
}