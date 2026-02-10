using System.ComponentModel;

namespace Musicx.Contracts.Enums;

public enum GenreType
{
    [Description("Defines a characteristic or a trait")]
    Descriptor = 1,
    
    [Description("The main genre")]
    Genre = 5,
    
    [Description("A fusion of different sub-genres or genres")]
    Fusion = 4,
    
    [Description("A localized sub-genre")]
    Scene = 2,
    
    [Description("A descendant of a main genre")]
    Subgenre = 0,
    
    [Description("A sub-genre that is or was active in a specific period.")]
    Movement = 3,
    
    [Description("A localization genre that serves to localize a specific sub-genre. Used for example in folk or classical music.")]
    Localization = 6,
}