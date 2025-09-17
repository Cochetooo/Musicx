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
        
        // Transformer PascalCase → snake_case
        modelName = System.Text.RegularExpressions.Regex
            .Replace(modelName, "([a-z0-9])([A-Z])", "$1-$2")
            .ToLower();

        // Remplacement spécifique
        modelName = modelName
            .ToLower()
            .Replace("attributes", "attrs");
        
        return modelName;
    }
    
    public static string OutModelToEntity(this string name)
        => InModelToEntity(name[1..]);
}