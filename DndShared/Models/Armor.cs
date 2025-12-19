namespace DndShared.Models;

public class Armor
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Category { get; set; }
    public string? ArmorClass { get; set; }
    public string? Strength { get; set; }
    public string? Stealth { get; set; }
    public string? Weight { get; set; }
    public string? Cost { get; set; }
    public string? DonTime { get; set; }
    public string? DoffTime { get; set; }

    public Armor()
    {
        Id = Guid.NewGuid();
    }

    public override string ToString()
    {
        return $"{Name} (AC: {ArmorClass}, {Category})";
    }
}
