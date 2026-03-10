using System.Text;

namespace Musicx.Application.Shared.Helpers;

public static class EnglishTitleCaseHelper
{
    private static readonly HashSet<string> MinorWords =
    [
        "a", "an", "the",
        "and", "but", "or", "nor", "for", "so", "yet",
        "as", "at", "by", "in", "of", "off", "on", "out", "per", "to", "up", "via"
    ];
    
    private static readonly HashSet<string> ForcedLowerWords = ["vs", "vs.", "v", "v."];
    
    public static bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        return value == ToTitleCase(value);
    }
    
    public static string ToTitleCase(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value ?? string.Empty;
        }

        var words = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        for (var i = 0; i < words.Length; i++)
        {
            var isFirst = i == 0;
            var isLast = i == words.Length - 1;
            words[i] = FormatToken(words[i], isFirst, isLast);
        }

        return string.Join(' ', words);
    }
    
    private static string FormatToken(string token, bool isFirstWord, bool isLastWord)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return token;
        }

        var prefixLength = 0;
        while (prefixLength < token.Length && !char.IsLetterOrDigit(token[prefixLength]))
        {
            prefixLength++;
        }

        var suffixLength = 0;
        while (suffixLength < token.Length - prefixLength &&
               !char.IsLetterOrDigit(token[token.Length - 1 - suffixLength]))
        {
            suffixLength++;
        }

        if (prefixLength + suffixLength >= token.Length)
        {
            return token;
        }

        var prefix = token[..prefixLength];
        var coreLength = token.Length - prefixLength - suffixLength;
        var core = token.Substring(prefixLength, coreLength);
        var suffix = token[(prefixLength + coreLength)..];

        var loweredCore = core.ToLowerInvariant();

        if (ForcedLowerWords.Contains(loweredCore))
        {
            return prefix + loweredCore + suffix;
        }

        if (!isFirstWord && !isLastWord && MinorWords.Contains(loweredCore))
        {
            return prefix + loweredCore + suffix;
        }

        return prefix + CapitalizeSegments(loweredCore) + suffix;
    }

    private static string CapitalizeSegments(string value)
    {
        var result = new StringBuilder(value.Length);
        var shouldUpper = true;

        foreach (var c in value)
        {
            if (shouldUpper && char.IsLetter(c))
            {
                result.Append(char.ToUpperInvariant(c));
                shouldUpper = false;
                continue;
            }

            result.Append(c);
            shouldUpper = c is '-' or '/';
        }

        return result.ToString();
    }
}