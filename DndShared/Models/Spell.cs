namespace DndShared.Models;

public class Spell
{
    public Guid Id { get; set; }
    public int Level { get; set; }
    public string? Name { get; set; }
    public string? School { get; set; }
    public string? CastingTime { get; set; }
    public string? Range { get; set; }
    public string? Duration { get; set; }
    public string? Components { get; set; }
    public string? Description { get; set; }
    public string? Source { get; set; }
    public string? AtHigherLevels { get; set; }
    public List<string> SpellLists { get; set; }

    public Spell()
    {
        Id = Guid.NewGuid();
        SpellLists = new List<string>();
    }

    public override string ToString()
    {
        return $"{Name} ({School}) - {CastingTime} | Range: {Range} | Duration: {Duration}";
    }
}