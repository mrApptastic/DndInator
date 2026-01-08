using Xunit;
using DndShared.Helpers;

namespace DndTests;

public class TextHelpersTests
{
    [Fact]
    public void Truncate_NullOrEmpty_ReturnsEmptyString()
    {
        // Arrange & Act
        var result1 = TextHelpers.Truncate(null, 10);
        var result2 = TextHelpers.Truncate("", 10);
        var result3 = TextHelpers.Truncate("   ", 10);

        // Assert
        Assert.Equal(string.Empty, result1);
        Assert.Equal(string.Empty, result2);
        Assert.Equal(string.Empty, result3);
    }

    [Fact]
    public void Truncate_ShorterThanMax_ReturnsOriginal()
    {
        // Arrange
        var text = "Short";

        // Act
        var result = TextHelpers.Truncate(text, 10);

        // Assert
        Assert.Equal("Short", result);
    }

    [Fact]
    public void Truncate_ExactlyMax_ReturnsOriginal()
    {
        // Arrange
        var text = "TenLetters";

        // Act
        var result = TextHelpers.Truncate(text, 10);

        // Assert
        Assert.Equal("TenLetters", result);
    }

    [Fact]
    public void Truncate_LongerThanMax_AppendsEllipsis()
    {
        // Arrange
        var text = "This is a very long text that should be truncated";

        // Act
        var result = TextHelpers.Truncate(text, 20);

        // Assert
        Assert.Equal(19 + 1, result.Length); // 19 chars + ellipsis
        Assert.EndsWith("…", result);
        Assert.StartsWith("This is a very long", result);
    }

    [Fact]
    public void Truncate_WithSpecialCharacters_TruncatesCorrectly()
    {
        // Arrange
        var text = "Paladin (Oath of Devotion) / Rogue (Thief)";

        // Act
        var result = TextHelpers.Truncate(text, 30);

        // Assert
        Assert.Equal(30, result.Length);
        Assert.EndsWith("…", result);
    }

    [Fact]
    public void JoinNonEmpty_NullList_ReturnsEmptyString()
    {
        // Arrange & Act
        var result = TextHelpers.JoinNonEmpty(null);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void JoinNonEmpty_EmptyList_ReturnsEmptyString()
    {
        // Arrange
        var list = new List<string>();

        // Act
        var result = TextHelpers.JoinNonEmpty(list);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void JoinNonEmpty_WithNullsAndEmpties_SkipsThem()
    {
        // Arrange
        var list = new List<string?> { "First", null, "", "  ", "Second" };

        // Act
        var result = TextHelpers.JoinNonEmpty(list);

        // Assert
        Assert.Equal("First, Second", result);
    }

    [Fact]
    public void JoinNonEmpty_CustomDelimiter_UsesCorrectDelimiter()
    {
        // Arrange
        var list = new List<string> { "Paladin", "Rogue", "Wizard" };

        // Act
        var result = TextHelpers.JoinNonEmpty(list, " / ");

        // Assert
        Assert.Equal("Paladin / Rogue / Wizard", result);
    }

    [Fact]
    public void JoinNonEmpty_AllNullOrEmpty_ReturnsEmptyString()
    {
        // Arrange
        var list = new List<string?> { null, "", "  " };

        // Act
        var result = TextHelpers.JoinNonEmpty(list);

        // Assert
        Assert.Equal(string.Empty, result);
    }
}
