namespace DndShared.Models;

public class Character
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public CharacterInformation? Information { get; set; } = null;
    public Species? Race { get; set; } = null;
    public CharacterClass? Class { get; set; } = null;
    public CharacterStats? Stats { get; set; } = null;
    public Background? Background { get; set; } = null;
    public List<Feat>? Feats { get; set; } = new List<Feat>();
}
