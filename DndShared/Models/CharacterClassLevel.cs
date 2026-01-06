namespace DndShared.Models;

/// <summary>
/// Represents a class level for a character, supporting multiclassing.
/// </summary>
public class CharacterClassLevel
{
    public CharacterClass? Class { get; set; } = null;
    public int Level { get; set; } = 1;
    public Subclass? Subclass { get; set; } = null;
    public List<string>? ClassFeatures { get; set; } = new List<string>();
}
