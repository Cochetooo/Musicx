namespace Musicx.Infrastructure.Web.Helpers;

public static class ModelNameHelper
{
    public static string InModelToEntity(this string name)
    {
        var modelName = name[2..];

        if (modelName.EndsWith("y", StringComparison.OrdinalIgnoreCase))
        {
            modelName = modelName[..^1] + "ies";
        }
        else
        {
            modelName += "s";
        }
        
        return modelName.ToLower();
    }
    
    public static string OutModelToEntity(this string name)
        => InModelToEntity(name[1..]);
}