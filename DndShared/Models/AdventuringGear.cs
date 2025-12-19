namespace DndShared.Models;

public class AdventuringGear
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Weight { get; set; }
    public string? Cost { get; set; }
    public string? Function { get; set; }

    public AdventuringGear()
    {
        Id = Guid.NewGuid();
    }

    public override string ToString()
    {
        return $"{Name} ({Cost})";
    }
}
