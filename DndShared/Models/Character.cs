namespace DndShared.Models;

public class Character
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public CharacterInformation? Information { get; set; } = null;
    public Species? Race { get; set; } = null;
    public List<CharacterClassLevel>? Classes { get; set; } = new List<CharacterClassLevel>();
    public CharacterStats? Stats { get; set; } = null;
    public Background? Background { get; set; } = null;
    public List<Feat>? Feats { get; set; } = new List<Feat>();
    
    /// <summary>
    /// Gets the total character level across all classes.
    /// </summary>
    public int GetTotalLevel() => Classes?.Sum(c => c.Level) ?? 1;
    
    /// <summary>
    /// Gets the first spell-casting class, if any.
    /// </summary>
    public CharacterClass? GetPrimarySpellcastingClass() 
        => Classes?.FirstOrDefault(c => !string.IsNullOrWhiteSpace(c.Class?.PrimaryAbility))?.Class;
}
