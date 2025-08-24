using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Shared.Utilities;

public static class GenreLabelHelper
{
    private static readonly Dictionary<string, string> Abbreviations = new()
    {
        { "Alternative", "Alt." },
        { "Progressive", "Prog" },
        { "Gothic", "Goth" },
        { "Melodic", "Melo" },
        { "Symphonic", "Sympho" },
        { "Electronic", "Electro." },
        { "Industrial", "Indus" },
        { "Psychedelic", "Psych." }
    };

    /// <summary>
    /// Simplifies and merges genres like "Alternative Rock", "Alternative Metal" into "Alt. Rock/Metal"
    /// </summary>
    public static string MergeGenres(List<string> rawGenres)
    {
        var simplifiedGroups = new Dictionary<string, List<string>>();

        foreach (var genre in rawGenres.Distinct())
        {
            var parts = genre.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 2)
            {
                var prefix = Abbreviations.TryGetValue(parts[0], out var abbr) ? abbr : parts[0];
                var suffix = parts[1];

                if (!simplifiedGroups.ContainsKey(prefix))
                {
                    simplifiedGroups[prefix] = [];
                }
                
                simplifiedGroups[prefix].Add(suffix);
            }
            else
            {
                simplifiedGroups.TryAdd(genre, []);
            }
        }

        var results = new List<string>();
        
        foreach (var (prefix, suffixes) in simplifiedGroups)
        {
            if (suffixes.Count == 0)
            {
                results.Add(prefix);
            }
            else
            {
                var joined = string.Join("/", suffixes.Distinct());
                results.Add($"{prefix} {joined}");
            }
        }

        return string.Join(" / ", results);
    }
    
    /// <summary>
    /// Entry point for genre objects
    /// </summary>
    public static string MergeGenresFromList(List<OutGenre> genres)
        => MergeGenres(genres.Select(g => g.Name).ToList());
}