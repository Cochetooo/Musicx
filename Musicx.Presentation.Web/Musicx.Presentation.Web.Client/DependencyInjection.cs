using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;

namespace Musicx.Presentation.Web.Client;

public static class DependencyInjection
{
    public static IServiceCollection AddFrontFramework(this IServiceCollection services)
    {
        services
            .AddBlazorise(options =>
            {
                options.Immediate = true;
                options.ProductToken =
                    "CjxRBXF5Nwo8WgNzejI1BlEAc3g1Dz5RAHZ/NA08bjoNJ2ZdYhBVCCo/CjtVCExERldhE1EvN0xcNm46FD1gSkUHCkxESVFv" +
                    "Bl4yK1FBfAYKAiFoVXkNWTU3CDJTPHQAGkR/Xip0HhFIeVQ8bxMBUmtTPApwfjUIPG46HhFEbVgscw4DVXRJN3UeEUh5VDxv" +
                    "EwFSa1M8CnB+NQg8bjoeEUZwTTFkEhFadU07bx4cSm9fPG97fzUIAWlvHgJMa1g1eQQZWmdBImgeEVd3WzBvHnQ0CDxTAExE" +
                    "WmdYMXUEGEx9WzxvDA9dZ1MxfxYdWmc2UgBxfggyWUgCNQFVcEFbV256c0pcAlILCHVfRhkbeGVyVkcrWiwhRm9nC3MOJ3Ne" +
                    "PSd5Dn9jWzlWZm5/dg5oDAMODUJdPzZ6eRdRUzsqeHk4MU1EGVwRdnF2YiZWJn5yVEgFSSM8QFpqLwcSBHxoYTteEX1zWyMK" +
                    "aDMIbmA0EgciHVJpXAJgCARfWWA7aSJ7UXI+B1R3LTF5bxlpAiZOT3k2fwxlM3NBGkkQCnJIPQ9xfA==";
            })
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons();
        
        return services;
    } 
}