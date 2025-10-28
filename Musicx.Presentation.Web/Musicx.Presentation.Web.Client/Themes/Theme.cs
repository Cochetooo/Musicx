using MudBlazor;

namespace Musicx.Presentation.Web.Client.Themes;

public static class Theme
{
    public static readonly MudTheme DeepTheme = new()
    {
        PaletteLight = new()
        {
            Primary = "#62b1ac",
            PrimaryDarken = "#589f99",

            Secondary = "#549f99",
            SecondaryDarken = "#468c85",
            
            Tertiary = "#387a73",
            TertiaryDarken = "256861",
            
            Surface = "#ffffff",
            Background = "#fafafa",
            BackgroundGray = "#efefef",
            AppbarBackground = "#7b9f99",
            
            SuccessDarken = "#22946e",
            Success = "#47d5a6",
            SuccessLighten = "9ae8ce",
            
            WarningDarken = "#a87a2a",
            Warning = "#d7ac61",
            WarningLighten = "#ecd7b2",
            
            ErrorDarken = "#9c2121",
            Error = "#d94a4a",
            ErrorLighten = "#eb9e9e",
            
            InfoDarken = "#21498a",
            Info = "#4077d1",
            InfoLighten = "#92b2e5"
        },

        PaletteDark = new()
        {
            Primary = "#62b1ac",
            PrimaryDarken = "#589f99",

            Secondary = "#549f99",
            SecondaryDarken = "#468c85",
            
            Tertiary = "#387a73",
            TertiaryDarken = "256861",
            
            Surface = "#3d3d3d",
            Background = "#282828",
            BackgroundGray = "#1f1f1f",

            TextPrimary = "#fcfcfc",
            
            SuccessDarken = "#22946e",
            Success = "#47d5a6",
            SuccessLighten = "9ae8ce",
            
            WarningDarken = "#a87a2a",
            Warning = "#d7ac61",
            WarningLighten = "#ecd7b2",
            
            ErrorDarken = "#9c2121",
            Error = "#d94a4a",
            ErrorLighten = "#eb9e9e",
            
            InfoDarken = "#21498a",
            Info = "#4077d1",
            InfoLighten = "#92b2e5"
        },
        
        Typography = new()
        {
            Default = new DefaultTypography
            {
                FontFamily = ["ClashGrotesk-Medium", "Segoe UI", "sans-serif"]
            },
            Body1 = new Body1Typography
            {
                FontFamily = ["ClashGrotesk-Medium", "Segoe UI", "sans-serif"]
            },
            Body2 = new Body2Typography
            {
                FontFamily = ["ClashGrotesk-Medium", "Segoe UI", "sans-serif"]
            }
        }
    };
    
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
            PrimaryLighten = Colors.Teal.Accent3,
            
            Secondary = Colors.Teal.Lighten3,
            SecondaryDarken = Colors.Teal.Lighten4,
            SecondaryLighten = Colors.Teal.Accent2,
            
            Tertiary = Colors.Teal.Lighten4,
            TertiaryDarken = Colors.Teal.Lighten5,
            
            TextPrimary = "#fbfbfb"
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