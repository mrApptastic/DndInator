namespace DndShared.Models;

public class Poison
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Cost { get; set; }
    public string? Effect { get; set; }

    public Poison()
    {
        Id = Guid.NewGuid();
    }

    public override string ToString()
    {
        return $"{Name} ({Type}, {Cost})";
    }
}
