
using DndInator.Services;

namespace DndTests;

public class DiceServiceTests
{
    [Fact]
    public void RollDice_ShouldReturnValueWithinRange()
    {
        // Arrange
        var diceService = new DiceService();
        int sides = 6;

        // Act
        int result = diceService.RollDice(sides);

        // Assert
        Assert.InRange(result, 1, sides);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(6)]
    [InlineData(8)]
    [InlineData(10)]
    [InlineData(12)]
    [InlineData(20)]
    [InlineData(100)]
    public void RollDice_WithDifferentSides_ShouldReturnValueWithinRange(int sides)
    {
        // Arrange
        var diceService = new DiceService();

        // Act
        int result = diceService.RollDice(sides);

        // Assert
        Assert.InRange(result, 1, sides);
    }

    [Fact]
    public void RollDice_D20_ShouldNeverExceed20()
    {
        // Arrange
        var diceService = new DiceService();

        // Act & Assert - Roll 1000 times to ensure consistency
        for (int i = 0; i < 1000; i++)
        {
            int result = diceService.RollDice(20);
            Assert.InRange(result, 1, 20);
        }
    }

    [Fact]
    public void RollDice_WithOneSide_ShouldAlwaysReturnOne()
    {
        // Arrange
        var diceService = new DiceService();

        // Act
        int result = diceService.RollDice(1);

        // Assert
        Assert.Equal(1, result);
    }
}
