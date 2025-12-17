namespace DndShared.Models;

public class MagicItem
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Category { get; set; }
    public string? Source { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? Rarity { get; set; }
    public string? Attunement { get; set; }
    public List<string> Properties { get; set; }
    public string? FullContent { get; set; }

    public MagicItem()
    {
        Id = Guid.NewGuid();
        Properties = new List<string>();
    }

    public override string ToString()
    {
        return $"{Name} ({Rarity})";
    }
}
