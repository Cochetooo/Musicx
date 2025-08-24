using MudBlazor;

namespace Musicx.Presentation.Web.Client.Themes;

public static class Theme
{
    public static readonly MudTheme MainTheme = new()
    {
        PaletteLight = new()
        {
            Primary = Colors.Teal.Default,
            PrimaryDarken = Colors.Teal.Darken1,
            PrimaryLighten = Colors.Teal.Accent3,
            
            Secondary = Colors.Teal.Darken2,
            SecondaryDarken = Colors.Teal.Darken3,
            SecondaryLighten = Colors.Teal.Accent4,
            
            Tertiary = Colors.Teal.Darken4,
            TertiaryLighten = Colors.Teal.Darken3,
            
            AppbarBackground = Colors.Teal.Darken1,
        },
        PaletteDark = new()
        {
            Primary = Colors.Teal.Lighten2,
            PrimaryDarken = Colors.Teal.Lighten1,
            PrimaryLighten = Colors.Teal.Accent2,
            
            Secondary = Colors.Teal.Lighten4,
            SecondaryDarken = Colors.Teal.Lighten3,
            SecondaryLighten = Colors.Teal.Accent1,
            
            Tertiary = Colors.Teal.Lighten5,
            TertiaryDarken = Colors.Teal.Lighten4,
            
            TextPrimary = "#fbfbfb"
        },
        
        LayoutProperties = new()
        {
            
        },
        
        PseudoCss = new()
        {
            
        },
        
        Typography = new()
        {
            Default = new DefaultTypography
            {
                FontFamily = ["Space Grotesk", "Segoe UI", "sans-serif"]
            },
            Body1 = new Body1Typography
            {
                FontFamily = ["Space Grotesk", "Segoe UI", "sans-serif"]
            },
            Body2 = new Body2Typography
            {
                FontFamily = ["Space Grotesk", "Segoe UI", "sans-serif"]
            }
        }
    };
}