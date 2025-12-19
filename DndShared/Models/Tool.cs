namespace DndShared.Models;

public class Tool
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Category { get; set; }
    public string? Ability { get; set; }
    public string? Weight { get; set; }
    public string? Cost { get; set; }

    public Tool()
    {
        Id = Guid.NewGuid();
    }

    public override string ToString()
    {
        return $"{Name} ({Category}, {Cost})";
    }
}
