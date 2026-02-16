namespace Musicx.Application.Shared.Helpers;

public static class SearchKeywordHelper
{
    public static List<T> FilterByKeywords<T>(
        IEnumerable<T> source,
        string? search,
        params Func<T, string?>[] selectors)
    {
        var keywords = SplitKeywords(search);
        if (keywords.Length == 0 || selectors.Length == 0)
        {
            return source.ToList();
        }

        return source.Where(item => MatchesAllKeywords(item, keywords, selectors)).ToList();
    }

    private static bool MatchesAllKeywords<T>(
        T item,
        string[] keywords,
        IReadOnlyList<Func<T, string?>> selectors)
    {
        return keywords.All(keyword =>
            selectors.Any(selector => selector(item)?.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true));
    }

    private static string[] SplitKeywords(string? search)
    {
        return string.IsNullOrWhiteSpace(search)
            ? []
            : search.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}