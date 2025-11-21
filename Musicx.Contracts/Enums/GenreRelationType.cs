namespace Musicx.Contracts.Enums;

public enum GenreRelationType
{
    // Parent <-> Child Hierarchy
    IsA = 0,
    
    // Historical influence
    InfluencedBy = 1,
    
    // Composition A + B with weighting
    FusionOf = 2,
    
    // Scene <-> Bands <-> Genres
    AssociatedWith = 3,
    
    // Synonyms, Alt. Name
    SameAs = 4,
    
    // Node Proprety
    TaggableFlag = 5,
    
    // Link Genre <-> Facet
    HasFacet = 6
}