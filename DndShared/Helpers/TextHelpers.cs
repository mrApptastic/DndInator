namespace DndShared.Helpers;

/// <summary>
/// Text utility methods for formatting and truncation
/// </summary>
public static class TextHelpers
{
    /// <summary>
    /// Truncates text to a maximum length, appending ellipsis if needed.
    /// </summary>
    /// <param name="text">The text to truncate</param>
    /// <param name="maxLength">Maximum length including ellipsis</param>
    /// <returns>Truncated text with ellipsis if longer than maxLength, or original text</returns>
    public static string Truncate(string? text, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;
        
        if (text.Length <= maxLength)
            return text;
        
        return text.Substring(0, maxLength - 1) + "…";
    }
    
    /// <summary>
    /// Joins a list of strings with a delimiter, skipping null or empty values.
    /// </summary>
    public static string JoinNonEmpty(IEnumerable<string?>? values, string delimiter = ", ")
    {
        if (values == null) return string.Empty;
        
        return string.Join(delimiter, values.Where(v => !string.IsNullOrWhiteSpace(v)));
    }
}
