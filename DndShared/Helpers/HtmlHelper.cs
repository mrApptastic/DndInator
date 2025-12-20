namespace DndShared.Helpers;

public static class HtmlHelper
{
    /// <summary>
    /// Decodes HTML entities in a string (e.g., &#160; becomes a non-breaking space).
    /// </summary>
    /// <param name="text">The text containing HTML entities to decode.</param>
    /// <returns>The decoded string, or empty string if input is null or empty.</returns>
    public static string DecodeHtml(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;
        
        return System.Net.WebUtility.HtmlDecode(text);
    }
}
